namespace ZME.API.Shared.Utils;

public class Constants
{
    public static readonly string[] SENIOR_STAFF_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM"
    };
    public const string SENIOR_STAFF = "ATM,DATM,TA,WM";

    public static readonly string[] FULL_STAFF_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM",
        "EC",
        "FE"
    };
    public const string FULL_STAFF = "ATM,DATM,TA,WM,EC,FE";

    public static readonly string[] ALL_STAFF_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "ATA",
        "WM",
        "AWM",
        "EC",
        "AEC",
        "FE",
        "AFE"
    };
    public const string ALL_STAFF = "ATM,DATM,TA,ATA,WM,AWM,EC,AEC,FE,AFE";

    public static readonly string[] SENIOR_TRAINING_STAFF_LIST = new string[]
    {
        "TA",
        "ATA",
        "INS",
        "WM"
    };
    public const string SENIOR_TRAINING_STAFF = "TA,ATA,INS,WN";

    public static readonly string[] TRAINING_STAFF_LIST = new string[]
    {
        "TA",
        "ATA",
        "INS",
        "MTR",
        "WM"
    };
    public const string TRAINING_STAFF = "TA,ATA,INS,MTR,WM";

    public const string CAN_REGISTER_FOR_EVENTS = "CanRegisterForEvents";
    public const string CAN_REQUEST_TRAINING = "CanRequestTraining";

    public static readonly string[] CAN_AIRPORTS_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM",
        "AWM",
        "FE",
        "AFE"
    };
    public const string CAN_AIRPORTS = "ATM,DATM,TA,WM,ATM,FE,AFE";

    public static readonly string[] CAN_MANAGE_CERTIFICATIONS_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM",
        "AWM"
    };
    public const string CAN_MANAGE_CERTIFICATIONS = "ATM,DATM,TA,WM,AWM";

    public static readonly string[] CAN_COMMENT_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "ATA",
        "WM",
        "AWM",
        "EC",
        "AEC",
        "FE",
        "AFE",
        "INS",
        "MTR"
    };
    public const string CAN_COMMENT = "ATM,DATM,TA,ATA,WM,AWM,EX,AEC,FE,AFE,INS,MTR";

    public static readonly string[] CAN_COMMENT_CONFIDENTIAL_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM"
    };
    public const string CAN_COMMENT_CONFIDENTIAL = "ATM,DATM,TA,WM";

    public static readonly string[] CAN_EMAIL_LOG_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM",
        "AWM"
    };
    public const string CAN_EMAIL_LOG = "ATM,DATM,TA,WM,AWM";

    public static readonly string[] CAN_EVENTS_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "WM",
        "EC",
        "AEC"
    };
    public const string CAN_EVENTS = "ATM,DATM,TA,WM,EC,AEC";

    public static readonly string[] CAN_FAQ_LIST = new string[]
    {
        "ATM",
        "DATM",
        "TA",
        "ATA",
        "WM",
        "AWM",
        "EC",
        "AEC",
        "FE",
        "AFE"
    };
    public const string CAN_FAQ = "ATM,DATM,TA,ATA,WM,AWM,EC,AEC,FE,AFE";
}
