using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorApp2.Shared
{
    public static class Logic
    {
        public static string AssignRandom(List<Participant> participants, int retryCount = 0)
        {
            if (retryCount >= 100)
            {
                Console.WriteLine("Exceeded retry count limit");
                return $"Exceeded retry count limit ({retryCount}). Aborting";
            }

            if (participants.Select(p => p.OwnNumber).Distinct().Count() != participants.Count())
            {
                participants.ForEach(p => p.OwnNumber = null);
                return "Noen skrev inn samme tall";
            }

            var random = new Random();
            var alreadyAssignedNumbers = participants.Where(p => p.AssignedNumber != null).Select(p => p.AssignedNumber);
            var availableNumbers = participants.Select(p => p.OwnNumber).Except(alreadyAssignedNumbers).ToList();

            foreach (var participant in participants.Where(p => p.AssignedNumber == null))
            {
                var candidateNumbers = availableNumbers.Where(n => n != participant.OwnNumber).ToList();
                if (candidateNumbers.Count == 0)
                {
                    Console.WriteLine("Retry");
                    return AssignRandom(participants, retryCount + 1);
                }
                var selection = candidateNumbers[random.Next(candidateNumbers.Count)] ?? -1;
                availableNumbers.Remove(selection);
                participant.CandidateNumber = selection;
            }
            foreach (var participant in participants.Where(p => p.AssignedNumber == null))
            {
                participant.AssignedNumber = participant.CandidateNumber;
            }

            var errorMessage = ErrorCheck(participants);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                participants.ForEach(p => p.OwnNumber = null);
                return errorMessage;
            }

            return null;
        }

        public static string ErrorCheck(List<Participant> participants)
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
            var ownNumbers = new HashSet<int?>(participants.Select(p => p.OwnNumber));
            var assignedNumbers = new HashSet<int?>(participants.Select(p => p.AssignedNumber));
            if (!ownNumbers.SetEquals(assignedNumbers))
            {
                return "Registrering er ulik tildelte nummer";
            }
            return null;
        }
    }
}
