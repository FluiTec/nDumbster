#region copyright
/*
 * nDumbster - a dummy SMTP server
 * Copyright 2005 Martin Woodward
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion // copyright

namespace nDumbsterCore.smtp
{
	/// <summary> Represents an SMTP action or command.</summary>
	internal class SmtpActionType
	{
		#region Members
		/// <summary>
		/// Internal value for the action type.
		/// </summary>
		private readonly sbyte _action;

		/// <summary>
		/// Internal representation of the CONNECT action.
		///  </summary>
		private const sbyte ConnectByte = 1;
		/// <summary>
		/// Internal representation of the EHLO action. 
		/// </summary>
		private const sbyte EhloByte = 2;
		/// <summary>
		/// Internal representation of the MAIL FROM action. 
		/// </summary>
		private const sbyte MailByte = 3;
		/// <summary>
		/// Internal representation of the RCPT action.
		///  </summary>
		private const sbyte RcptByte = 4;
		/// <summary>
		/// Internal representation of the DATA action.
		/// </summary>
		private const sbyte DataByte = 5;
		/// <summary>
		/// Internal representation of the DATA END (.) action.
		/// </summary>
		private const sbyte DataEndByte = 6;
		/// <summary>Internal representation of the QUIT action. </summary>
		private const sbyte QuitByte = 7;
		/// <summary>
		/// Internal representation of an unrecognized action: body text gets this action type.
		/// </summary>
		private const sbyte UnrecByte = 8;
		/// <summary>
		/// Internal representation of the blank line action: separates headers and body text.
		/// </summary>
		private const sbyte BlankLineByte = 9;

		/// <summary>
		/// Internal representation of the stateless RSET action.
		/// </summary>
		private const sbyte RsetByte = -1;
		/// <summary>
		/// Internal representation of the stateless VRFY action.
		/// </summary>
		private const sbyte VrfyByte = -2;
		/// <summary>
		/// Internal representation of the stateless EXPN action.
		/// </summary>
		private const sbyte ExpnByte = -3;
		/// <summary>
		/// Internal representation of the stateless HELP action.
		/// </summary>
		private const sbyte HelpByte = -4;
		/// <summary>
		/// Internal representation of the stateless NOOP action.
		/// </summary>
		private const sbyte NoopByte = -5;

		/// <summary>
		/// CONNECT action.
		/// </summary>
		public static readonly SmtpActionType CONNECT = new SmtpActionType(ConnectByte);
		/// <summary>
		/// EHLO action.
		/// </summary>
		public static readonly SmtpActionType EHLO = new SmtpActionType(EhloByte);
		/// <summary>
		/// MAIL action.
		/// </summary>
		public static readonly SmtpActionType MAIL = new SmtpActionType(MailByte);
		/// <summary>
		/// RCPT action.
		/// </summary>
		public static readonly SmtpActionType RCPT = new SmtpActionType(RcptByte);
		/// <summary>
		/// DATA action.
		/// </summary>
		public static readonly SmtpActionType DATA = new SmtpActionType(DataByte);
		/// <summary>
		/// "." action.
		/// </summary>
		public static readonly SmtpActionType DATA_END = new SmtpActionType(DataEndByte);
		/// <summary>
		/// Body text action.
		/// </summary>
		public static readonly SmtpActionType UNRECOG = new SmtpActionType(UnrecByte);
		/// <summary>
		/// QUIT action.
		/// </summary>
		public static readonly SmtpActionType QUIT = new SmtpActionType(QuitByte);
		/// <summary>
		/// Header/body separator action.
		/// </summary>
		public static readonly SmtpActionType BLANK_LINE = new SmtpActionType(BlankLineByte);

		/// <summary>
		/// Stateless RSET action.
		/// </summary>
		public static readonly SmtpActionType RSET = new SmtpActionType(RsetByte);
		/// <summary>
		/// Stateless VRFY action. 
		/// </summary>
		public static readonly SmtpActionType VRFY = new SmtpActionType(VrfyByte);
		/// <summary>
		/// Stateless EXPN action. 
		/// </summary>
		public static readonly SmtpActionType EXPN = new SmtpActionType(ExpnByte);
		/// <summary>
		/// Stateless HELP action. 
		/// </summary>
		public static readonly SmtpActionType HELP = new SmtpActionType(HelpByte);
		/// <summary>
		/// Stateless NOOP action. 
		/// </summary>
		public static readonly SmtpActionType NOOP = new SmtpActionType(NoopByte);

		#endregion // Members

		#region Contructors
		/// <summary>
		/// Create a new SMTP action type. Private to ensure no invalid values.
		/// </summary>
		/// <param name="action">one of the _BYTE values</param>
		private SmtpActionType(sbyte action)
		{
			_action = action;
		}
		#endregion // Constructors

		#region Properties
		/// <summary>
		/// Indicates whether the action is stateless or not.
		/// </summary>
		public virtual bool Stateless => _action < 0;

        #endregion // Properties

		/// <summary> 
		/// String representation of this SMTP action type.
		/// </summary>
		/// <returns>A String that represents the current SmtpActionType</returns>
		/// 
		public override string ToString()
		{
			switch (_action)
			{

				case ConnectByte:
					return "Connect";

				case EhloByte:
					return "EHLO";

				case MailByte:
					return "MAIL";

				case RcptByte:
					return "RCPT";

				case DataByte:
					return "DATA";

				case DataEndByte:
					return ".";

				case QuitByte:
					return "QUIT";

				case RsetByte:
					return "RSET";

				case VrfyByte:
					return "VRFY";

				case ExpnByte:
					return "EXPN";

				case HelpByte:
					return "HELP";

				case NoopByte:
					return "NOOP";

				case UnrecByte:
					return "Unrecognized command / data";

				case BlankLineByte:
					return "Blank line";

				default:
					return "Unknown";

			}
		}
	}
}
