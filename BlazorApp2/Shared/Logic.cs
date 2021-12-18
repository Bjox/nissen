using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorApp2.Shared
{
    public static class Logic
    {
        public static string AssignRandom(List<Participant> participants)
        {
            if (participants.Select(p => p.OwnNumber).Distinct().Count() != participants.Count())
            {
                participants.ForEach(p => p.OwnNumber = null);
                return "Noen skrev inn samme tall";
            }

            var nonPreAssignedParticipants = participants.Where(p => p.AssignedNumber == null).ToList();
            var preAssignments = participants.Where(p => p.AssignedNumber != null).ToDictionary(p => p.Name, p => p.AssignedNumber.Value);

            // assign everyone their own number
            foreach (var participant in participants)
            {
                participant.AssignedNumber = participant.OwnNumber;
            }

            // assign pre-assignments by swapping
            foreach (var assignment in preAssignments)
            {
                var participant = participants.Single(p => p.Name == assignment.Key);
                var participantToSwapWith = participants.Single(p => p.AssignedNumber == assignment.Value);
                participantToSwapWith.AssignedNumber = participant.AssignedNumber;
                participant.AssignedNumber = assignment.Value;
            }

            FisherYatesShuffle(nonPreAssignedParticipants);

            var errorMessage = ErrorCheck(participants, preAssignments);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                participants.ForEach(p => p.OwnNumber = null);
                return errorMessage;
            }

            return null;
        }

        private static void FisherYatesShuffle(List<Participant> participants)
        {
            // fisher-yates inspired shuffle, but cannot swap with itself
            var random = new Random();
            for (int i = 0; i < participants.Count - 1; i++)
            {
                var swapWith = random.Next(i + 1, participants.Count);
                var thisAssignedNumber = participants[i].AssignedNumber;
                participants[i].AssignedNumber = participants[swapWith].AssignedNumber;
                participants[swapWith].AssignedNumber = thisAssignedNumber;
            }
        }

        private static string ErrorCheck(List<Participant> participants, Dictionary<string, int> preAssignments)
        {
            if (participants.Any(p => p.OwnNumber == null))
            {
                return "Noen har ikke angitt tall";
            }
            if (participants.Any(p => p.AssignedNumber == null))
            {
                return "Noen har ikke fått gave";
            }
            if (participants.Any(p => p.OwnNumber == p.AssignedNumber))
            {
                return "Noen har fått sin egen gave";
            }
            if (participants.Select(p => p.OwnNumber).Distinct().Count() != participants.Count())
            {
                return "Noen skrev inn samme tall";
            }
            if (participants.Select(p => p.AssignedNumber).Distinct().Count() != participants.Count())
            {
                return "Noen har fått samme gave";
            }
            foreach (var preAssignment in preAssignments)
            {
                if (participants.Single(p => p.Name == preAssignment.Key).AssignedNumber != preAssignment.Value)
                {
                    return $"Tildeling ikke mulig ({preAssignment.Key})";
                }
            }

            var ownNumbers = new HashSet<int?>(participants.Select(p => p.OwnNumber));
            var assignedNumbers = new HashSet<int?>(participants.Select(p => p.AssignedNumber));
            if (!ownNumbers.SetEquals(assignedNumbers))
            {
                return "Internal error: assigned numbers does not match registered numbers";
            }
            return null;
        }
    }
}
