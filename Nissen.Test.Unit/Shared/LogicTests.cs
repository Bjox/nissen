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
                Logic.AssignRandom(participants).Should().BeNull();
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
        public void Test_WhenAssignedNumberIsPreSet()
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
                Logic.AssignRandom(participants).Should().BeNull();
            }
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
            Logic.AssignRandom(participants).Should().BeNull();
        }

        private List<Participant> CreateParticipants(params (string name, int ownNumber, int? assignedNumber)[] data)
        {
            return data.Select(p => new Participant(p.name)
            {
                OwnNumber = p.ownNumber,
                AssignedNumber = p.assignedNumber
            }).ToList();
        }
    }
}
