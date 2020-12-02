using System.ComponentModel;

namespace SysInfo_Fiskalizator.Model
{
    public enum MessageType
    {
        [Description("Ostalo")]
        Other = 0,

        [Description("EchoRequest")]
        Echo_To_CIS = 10,
        [Description("EchoReply")]
        Echo_From_CIS = 11,

        [Description("PoslovniProstorZahtjev")]
        Workspace_To_CIS = 20,
        [Description("PoslovniProstorOdgovor")]
        Workspace_From_CIS = 21,

        [Description("RacunZahtjev")]
        Invoice_To_CIS = 30,
        [Description("RacunOdgovor")]
        Invoice_From_CIS = 31,

        [Description("ProvjeraZahtjev")]
        Check_To_CIS = 40,
        [Description("ProvjeraOdgovor")]
        Check_From_CIS = 41
    }
}