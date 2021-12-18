using BlazorApp2.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nissen.Test.Unit.Shared
{
    [TestClass]
    public class LogicTests
    {
        private string RunAssignmentFunction(List<Participant> participants) => Logic.AssignRandom(participants);

        [TestMethod]
        public void Test_StackOverflowWhenRetrying()
        {
            for (int i = 0; i < 1000; i++)
            {
                var participants = CreateParticipants(
                    ("a", 1, 2),
                    ("b", 2, null),
                    ("c", 3, null),
                    ("d", 4, null),
                    ("e", 5, null),
                    ("f", 6, null)
                );
                RunAssignmentFunction(participants).Should().BeNull();
            }
        }

        [TestMethod]
        public void Test_DifferentNumberOfParticipants()
        {
            for (int participants = 4; participants < 20; participants++)
            {
                for (int rep = 0; rep < 1000; rep++)
                {
                    TestWithNParticipants(participants);
                }
            }
        }

        [TestMethod]
        public void Test_WhenPreAssignedNumber()
        {
            for (int i = 0; i < 1000; i++)
            {
                var participants = CreateParticipants(
                    ("a", 1, 2),
                    ("b", 2, null),
                    ("c", 3, 5),
                    ("d", 4, null),
                    ("e", 5, null),
                    ("f", 6, 1)
                );
                RunAssignmentFunction(participants).Should().BeNull();
                participants.Single(p => p.Name == "a").AssignedNumber.Should().Be(2);
                participants.Single(p => p.Name == "c").AssignedNumber.Should().Be(5);
                participants.Single(p => p.Name == "f").AssignedNumber.Should().Be(1);
            }
        }

        [TestMethod]
        public void Test_WhenDuplicatePreAssignment()
        {
            var participants = CreateParticipants(
                    ("a", 1, 2),
                    ("b", 2, null),
                    ("c", 3, 2),
                    ("d", 4, null),
                    ("e", 5, null),
                    ("f", 6, null)
                );
            RunAssignmentFunction(participants).Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Test_Fairness()
        {
            var counter = new Counter(6);
            for (int i = 0; i < 100000; i++)
            {
                var participants = CreateParticipants(
                    ("a", 0, null),
                    ("b", 1, null),
                    ("c", 2, null),
                    ("d", 3, null),
                    ("e", 4, null),
                    ("f", 5, null)
                );
                RunAssignmentFunction(participants);
                counter.Register(participants);
            }
            counter.CheckFairness();
        }

        private void TestWithNParticipants(int n)
        {
            var participants = new List<Participant>(n);
            for (int i = 0; i < n; i++)
            {
                participants.Add(new Participant(Guid.NewGuid().ToString())
                {
                    OwnNumber = i
                });
            }
            RunAssignmentFunction(participants).Should().BeNull();
        }

        private List<Participant> CreateParticipants(params (string name, int ownNumber, int? assignedNumber)[] data)
        {
            return data.Select(p => new Participant(p.name)
            {
                OwnNumber = p.ownNumber,
                AssignedNumber = p.assignedNumber
            }).ToList();
        }

        class Counter
        {
            public Dictionary<string, Dictionary<string, int>> Counts { get; }
            private readonly int _numParticipants;
            private int _numberOfRuns = 0;

            public Counter(int numParticipants)
            {
                _numParticipants = numParticipants;
                Counts = new Dictionary<string, Dictionary<string, int>>();
            }

            public void Register(List<Participant> participants)
            {
                if (participants.Count != _numParticipants)
                {
                    throw new ArgumentException();
                }
                foreach (var participant in participants)
                {
                    AssureParticipant(participant, participants);
                    Counts[participant.Name][participants.Single(p => p.OwnNumber == participant.AssignedNumber).Name]++;
                }
                _numberOfRuns++;
            }

            public void CheckFairness()
            {
                var tolerence = 0.05;
                var expectation = _numberOfRuns / (_numParticipants - 1.0);
                foreach (var participant in Counts)
                {
                    foreach (var count in participant.Value)
                    {
                        if (count.Key == participant.Key)
                        {
                            count.Value.Should().Be(0);
                        }
                        else
                        {
                            (count.Value / expectation).Should().BeApproximately(1, tolerence);
                        }
                    }
                }
            }

            private void AssureParticipant(Participant participant, List<Participant> allParticipants)
            {
                if (!Counts.ContainsKey(participant.Name))
                {
                    var dict = new Dictionary<string, int>();
                    foreach (var p in allParticipants)
                    {
                        dict[p.Name] = 0;
                    }
                    Counts[participant.Name] = dict;
                }
            }
        }
    }
}
