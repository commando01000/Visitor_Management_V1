namespace Visitor_Management_Portal.Helpers
{
    public static class VMHelpers
    {
        public static string GetZonesString(int count)
        {
            if (count == 0)
                return string.Empty;

            return $"{count} Zone{(count == 1 ? "" : "s")}";
        }

        public static string GetMeetingAreaString(int count)
        {
            if (count == 0)
                return string.Empty;

            return $"{count} Meeting Area{(count == 1 ? "" : "s")}";
        }

        public static string FormatStatus(string status)
        {
            return status.Replace("AutoClosed", "Auto-Closed")
                         .Replace("InProgress", "In Progress") // Add more mappings if needed
                         .Replace("PendingApproval", "Pending Approval")
                         .Replace("MeetingCancelled", "Meeting Cancelled")
                         .Replace("MeetingArea", "Meeting Area");
        }
    }
}