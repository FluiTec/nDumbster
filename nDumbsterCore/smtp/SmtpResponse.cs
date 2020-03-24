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
	/// SMTP response container.
	/// </summary>
	internal class SmtpResponse
	{

		#region Members

        #endregion // Members

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="code">response code</param>
		/// <param name="message">response message</param>
		/// <param name="next">next state of the SMTP server</param>
		public SmtpResponse(int code, string message, SmtpState next)
		{
			Code = code;
			Message = message;
			NextState = next;
		}
		#endregion // Constructor

		#region Properties
		/// <summary> 
		/// Response code.
		/// </summary>
		public virtual int Code { get; }

        /// <summary> 
		/// Response message.
		/// </summary>
		public virtual string Message { get; }

        /// <summary>
		/// Next SMTP server state.
		/// </summary>
		public virtual SmtpState NextState { get; }

        #endregion // Properties
	}
}
