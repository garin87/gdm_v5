
namespace gdm5._0.Shared.Enums
{
    public enum StatusCodeEnum
    {
        Unknown = 0,
        Success = 200,
        NotFound = 401,
        IncorrectEmailOrPassword = 402,
        TokenInvalid = 406,
        DBError = 501,
        CannotGetFullDashboardInfo = 502,
        MailFailed = 511,
        Duplicate = 550,
        InvalidXmlFile = 601,
        IncorrectFacilityPbjId = 602,
        IncorrectChartColemnsRange = 603,

        FSEmailSendingFailure = 700,
        ReportGenerationFailure = 701,
        FSEmailSentButHasReportError = 702,
        HasReportError = 703,
        FileSystemSavingFailure = 801,
        Fail = 900,

        CustomMessage = 999
    }
}
