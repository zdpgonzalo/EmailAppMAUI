namespace MailAppMAUI.General
{
    public enum EnumCore
    {
        None,
        // DatBase   = EnumModules.Core << OpMask.Size,
        DatBase = OpMask.Next,
        AppData = DatBase + OpMask.Next,
        DatInfo = AppData + OpMask.Next,
        GenInfo = DatInfo + OpMask.Next,
        TabInfo = GenInfo + OpMask.Next,
        TabData = TabInfo + OpMask.Next,
        VarInfo = TabData + OpMask.Next,
        KeyInfo = VarInfo + OpMask.Next,
        RelInfo = KeyInfo + OpMask.Next,
        XmlTables = RelInfo + OpMask.Next,
        Last = XmlTables + OpMask.Next
    }

    public enum EnumData
    {
        None,
        //         Session    = EnumModules.Data << OpMask.Size,
        Session = EnumCore.Last,
        DbAccess = Session + OpMask.Next,
        DbSqlGen = DbAccess + OpMask.Next,
        DbRegen = DbSqlGen + OpMask.Next,
        ComInter = DbRegen + OpMask.Next,
        Last = DbRegen + OpMask.Next,
    }

    public enum EnumComm
    {
        None,
        // ComData    = EnumModules.Comm << OpMask.Size,
        ComData = EnumData.Last,
        ComInfo = ComData + OpMask.Next,
        TcpServer = ComInfo + OpMask.Next,
        ConManager = TcpServer + OpMask.Next,
        TcpRegIp = ConManager + OpMask.Next,
        Last = TcpRegIp + OpMask.Next
    }

    public enum DataModules
    {
        Last = EnumComm.Last
    };
}
