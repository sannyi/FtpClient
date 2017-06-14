using System;
using System.Text;

namespace FtpClientSample
{
    internal class FtpResponse
    {
        public string[] Commands { get; set; }
        public uint ReplyCode { get; set; }
        public uint DataPort { get; set; }
        public uint FileLength { get; set; }

        internal FtpResponse(string[] commands)
        {
           // if (commands == null)  { throw new ArgumentNullException("commands"); } Commands = commands;  
            Commands = commands ?? throw new ArgumentNullException("commands"); //Visual Studio 2017 correction, is this statement equal to the upper statement?
            ParseCommands();
        }

        private void ParseCommands()
        {
            foreach (var command in Commands)
            {
                string codeString = command.Substring(0, 3);
                uint code; // can we use the code variable again for file lenght (space saving)
                //can we use shorts for indexes?
                if (UInt32.TryParse(codeString, out code))
                {
                    ReplyCode = code;
                    switch (code)
                    {
                        case 213:
                            index = command.IndexOf(" ");
                            if (index < 0) return;

                            uint fileLength;
                            if (UInt32.TryParse(command.Substring(index + 1), out fileLength))
                                FileLength = fileLength;
                            break;

                        case 229:
                            int prefixIndex = command.IndexOf("|||");
                            int postFixIndex = command.IndexOf("|", prefixIndex + 3);

                            if (prefixIndex < 0 || postFixIndex < 0) return;
                            uint port;
                            if (UInt32.TryParse(command.Substring(prefixIndex + 3, postFixIndex - prefixIndex - 3), out port)) { DataPort = port; }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var command in Commands)
            {
                builder.AppendLine(command);
            }
            return builder.ToString();
        }
    }
}
