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

	/// <summary> 
	/// SMTP server state.
	/// </summary>
	internal class SmtpState
	{
		/// <summary>Internal representation of the state. </summary>
		private readonly sbyte _state;

		/// <summary>Internal representation of the CONNECT state. </summary>
		private const sbyte ConnectByte = 1;
		/// <summary>Internal representation of the GREET state. </summary>
		private const sbyte GreetByte = 2;
		/// <summary>Internal representation of the MAIL state. </summary>
		private const sbyte MailByte = 3;
		/// <summary>Internal representation of the RCPT state. </summary>
		private const sbyte RcptByte = 4;
		/// <summary>Internal representation of the DATA_HEADER state. </summary>
		private const sbyte DataHeaderByte = 5;
		/// <summary>Internal representation of the DATA_BODY state. </summary>
		private const sbyte DataBodyByte = 6;
		/// <summary>Internal representation of the QUIT state. </summary>
		private const sbyte QuitByte = 7;

		/// <summary>CONNECT state: waiting for a client connection. </summary>
		public static readonly SmtpState CONNECT = new SmtpState(ConnectByte);

		/// <summary>GREET state: wating for a ELHO message. </summary>
		public static readonly SmtpState GREET = new SmtpState(GreetByte);

		/// <summary>MAIL state: waiting for the MAIL FROM: command. </summary>
		public static readonly SmtpState MAIL = new SmtpState(MailByte);

		/// <summary>RCPT state: waiting for a RCPT &lt;email address&gt; command. </summary>
		public static readonly SmtpState RCPT = new SmtpState(RcptByte);

		/// <summary>Waiting for headers. </summary>
		public static readonly SmtpState DATA_HDR = new SmtpState(DataHeaderByte);
		
		/// <summary>Processing body text. </summary>
		public static readonly SmtpState DATA_BODY = new SmtpState(DataBodyByte);

		/// <summary>End of client transmission. </summary>
		public static readonly SmtpState QUIT = new SmtpState(QuitByte);

		/// <summary>
		/// Create a new SmtpState object. Private to ensure that only valid states can be created.
		/// </summary>
		/// <param name="state">one of the _BYTE values.</param>
		private SmtpState(sbyte state)
		{
			_state = state;
		}

		/// <summary>
		/// String representation of this SmtpState.
		/// </summary>
		/// <returns>A String that represents the current SmtpState</returns>
		public override string ToString()
		{
			switch (_state)
			{

				case ConnectByte:
					return "CONNECT";

				case GreetByte:
					return "GREET";

				case MailByte:
					return "MAIL";

				case RcptByte:
					return "RCPT";

				case DataHeaderByte:
					return "DATA_HDR";

				case DataBodyByte:
					return "DATA_BODY";

				case QuitByte:
					return "QUIT";

				default:
					return "Unknown";
			}
		}
	}

}
