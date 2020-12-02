using System;

namespace SysInfo_Fiskalizator.Util
{
    /// <summary>
    /// Exception koji u kombinaciji s bezuvjetnim try-catch-om emulira globlanu return naredbu, a pozvanu iz bilo koje dubine neke metode
    /// </summary>
    public class MyExitException : Exception
    {

    }
}