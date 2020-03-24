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
	/// Contains an SMTP client request and handles state transitions.
	/// </summary>
	/// <remarks>
	/// State transitions are handled using the following state transition table.
	/// <code>
	/// -----------+-------------------------------------------------------------------------------------------------
	/// |                                 State
	/// Action     +-------------+-----------+-----------+--------------+---------------+---------------+------------
	///			   | CONNECT     | GREET     | MAIL      | RCPT         | DATA_HDR      | DATA_BODY     | QUIT
	/// -----------+-------------+-----------+-----------+--------------+---------------+---------------+------------
	/// connect    | 220/GREET   | 503/GREET | 503/MAIL  | 503/RCPT     | 503/DATA_HDR  | 503/DATA_BODY | 503/QUIT
	/// ehlo       | 503/CONNECT | 250/MAIL  | 503/MAIL  | 503/RCPT     | 503/DATA_HDR  | 503/DATA_BODY | 503/QUIT
	/// mail       | 503/CONNECT | 503/GREET | 250/RCPT  | 503/RCPT     | 503/DATA_HDR  | 503/DATA_BODY | 250/RCPT
	/// rcpt       | 503/CONNECT | 503/GREET | 503/MAIL  | 250/RCPT     | 503/DATA_HDR  | 503/DATA_BODY | 503/QUIT
	/// data       | 503/CONNECT | 503/GREET | 503/MAIL  | 354/DATA_HDR | 503/DATA_HDR  | 503/DATA_BODY | 503/QUIT
	/// data_end   | 503/CONNECT | 503/GREET | 503/MAIL  | 503/RCPT     | 250/QUIT      | 250/QUIT      | 503/QUIT
	/// unrecog    | 500/CONNECT | 500/GREET | 500/MAIL  | 500/RCPT     | ---/DATA_HDR  | ---/DATA_BODY | 500/QUIT
	/// quit       | 503/CONNECT | 503/GREET | 503/MAIL  | 503/RCPT     | 503/DATA_HDR  | 503/DATA_BODY | 250/CONNECT
	/// blank_line | 503/CONNECT | 503/GREET | 503/MAIL  | 503/RCPT     | ---/DATA_BODY | ---/DATA_BODY | 503/QUIT
	/// rset       | 250/GREET   | 250/GREET | 250/GREET | 250/GREET    | 250/GREET     | 250/GREET     | 250/GREET
	/// vrfy       | 252/CONNECT | 252/GREET | 252/MAIL  | 252/RCPT     | 252/DATA_HDR  | 252/DATA_BODY | 252/QUIT
	/// expn       | 252/CONNECT | 252/GREET | 252/MAIL  | 252/RCPT     | 252/DATA_HDR  | 252/DATA_BODY | 252/QUIT
	/// help       | 211/CONNECT | 211/GREET | 211/MAIL  | 211/RCPT     | 211/DATA_HDR  | 211/DATA_BODY | 211/QUIT
	/// noop       | 250/CONNECT | 250/GREET | 250/MAIL  | 250/RCPT     | 250|DATA_HDR  | 250/DATA_BODY | 250/QUIT
	/// </code>
	/// </remarks>
	internal class SmtpRequest
	{
		/// <summary>
		/// Parameters of this request (remainder of command line once the command is removed.
		/// </summary>
		public virtual string Params { get; }

        /// <summary>
		/// SMTP action received from client. 
		/// </summary>
		private readonly SmtpActionType _action;
		/// <summary>
		/// Current state of the SMTP state table. 
		/// </summary>
		private readonly SmtpState _state;

        /// <summary> 
		/// Create a new SMTP client request.
		/// </summary>
		/// <param name="actionType">type of action/command</param>
		/// <param name="requestParams">remainder of command line once command is removed</param>
		/// <param name="state">current SMTP server state</param>
		public SmtpRequest(SmtpActionType actionType, string requestParams, SmtpState state)
		{
			_action = actionType;
			_state = state;
			Params = requestParams;
		}

		/// <summary> 
		/// Execute the SMTP request returning a response. This method models the state transition table for the SMTP server.
		/// </summary>
		/// <returns>Reponse to the request</returns>
		public virtual SmtpResponse Execute()
		{
            // ReSharper disable once RedundantAssignment
            SmtpResponse response = null;
			if (_action.Stateless)
			{
				if (SmtpActionType.EXPN == _action || SmtpActionType.VRFY == _action)
				{
					response = new SmtpResponse(252, "Not supported", _state);
				}
				else if (SmtpActionType.HELP == _action)
				{
					response = new SmtpResponse(211, "No help available", _state);
				}
				else if (SmtpActionType.NOOP == _action)
				{
					response = new SmtpResponse(250, "OK", _state);
				}
				else if (SmtpActionType.VRFY == _action)
				{
					response = new SmtpResponse(252, "Not supported", _state);
				}
				else if (SmtpActionType.RSET == _action)
				{
					response = new SmtpResponse(250, "OK", SmtpState.GREET);
				}
				else
				{
					response = new SmtpResponse(500, "Command not recognized", _state);
				}
			}
			else
			{
				// Stateful commands
				if (SmtpActionType.CONNECT == _action)
                {
                    response = SmtpState.CONNECT == _state ? new SmtpResponse(220, "localhost nDumbster SMTP service ready", SmtpState.GREET) : new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
                }
				else if (SmtpActionType.EHLO == _action)
                {
                    response = SmtpState.GREET == _state ? new SmtpResponse(250, "OK", SmtpState.MAIL) : new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
                }
				else if (SmtpActionType.MAIL == _action)
				{
					if (SmtpState.MAIL == _state || SmtpState.QUIT == _state)
					{
						response = new SmtpResponse(250, "OK", SmtpState.RCPT);
					}
					else
					{
						response = new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
					}
				}
				else if (SmtpActionType.RCPT == _action)
                {
                    response = SmtpState.RCPT == _state ? new SmtpResponse(250, "OK", _state) : new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
                }
				else if (SmtpActionType.DATA == _action)
                {
                    response = SmtpState.RCPT == _state ? new SmtpResponse(354, "Start mail input; end with <CRLF>.<CRLF>", SmtpState.DATA_HDR) : new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
                }
				else if (SmtpActionType.UNRECOG == _action)
				{
					if (SmtpState.DATA_HDR == _state || SmtpState.DATA_BODY == _state)
					{
						response = new SmtpResponse(-1, "", _state);
					}
					else
					{
						response = new SmtpResponse(500, "Command not recognized", _state);
					}
				}
				else if (SmtpActionType.DATA_END == _action)
				{
					if (SmtpState.DATA_HDR == _state || SmtpState.DATA_BODY == _state)
					{
						response = new SmtpResponse(250, "OK", SmtpState.QUIT);
					}
					else
					{
						response = new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
					}
				}
				else if (SmtpActionType.BLANK_LINE == _action)
				{
					if (SmtpState.DATA_HDR == _state)
					{
						response = new SmtpResponse(-1, "", SmtpState.DATA_BODY);
					}
					else if (SmtpState.DATA_BODY == _state)
					{
						response = new SmtpResponse(-1, "", _state);
					}
					else
					{
						response = new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
					}
				}
				else if (SmtpActionType.QUIT == _action)
                {
                    response = SmtpState.QUIT == _state ? new SmtpResponse(221, "localhost nDumbster service closing transmission channel", SmtpState.CONNECT) : new SmtpResponse(503, "Bad sequence of commands: " + _action, _state);
                }
				else
				{
					response = new SmtpResponse(500, "Command not recognized", _state);
				}
			}
			return response;
		}

		/// <summary>
		///  Create an SMTP request object given a line of the input stream from the client and the current internal state.
		///  </summary>
		/// <param name="s">line of input</param>
		/// <param name="state">current state</param>
		/// <returns>A populated SmtpRequest object</returns>
		public static SmtpRequest CreateRequest(string s, SmtpState state)
		{
            // ReSharper disable once RedundantAssignment
            SmtpActionType action = null;
			string requestParams = null;

			if (state == SmtpState.DATA_HDR)
			{
				if (s.Equals("."))
				{
					action = SmtpActionType.DATA_END;
				}
				else if (s.Length < 1)
				{
					action = SmtpActionType.BLANK_LINE;
				}
				else
				{
					action = SmtpActionType.UNRECOG;
					requestParams = s;
				}
			}
			else if (state == SmtpState.DATA_BODY)
			{
				if (s.Equals("."))
				{
					action = SmtpActionType.DATA_END;
				}
				else
				{
					action = SmtpActionType.UNRECOG;
					requestParams = s;
				}
			}
			else
			{
				string su = s.ToUpper();
				if (su.StartsWith("EHLO ") || su.StartsWith("HELO"))
				{
					action = SmtpActionType.EHLO;
					requestParams = s.Substring(5);
				}
				else if (su.StartsWith("MAIL FROM:"))
				{
					action = SmtpActionType.MAIL;
					requestParams = s.Substring(10);
				}
				else if (su.StartsWith("RCPT TO:"))
				{
					action = SmtpActionType.RCPT;
					requestParams = s.Substring(8);
				}
				else if (su.StartsWith("DATA"))
				{
					action = SmtpActionType.DATA;
				}
				else if (su.StartsWith("QUIT"))
				{
					action = SmtpActionType.QUIT;
				}
				else if (su.StartsWith("RSET"))
				{
					action = SmtpActionType.RSET;
				}
				else if (su.StartsWith("NOOP"))
				{
					action = SmtpActionType.NOOP;
				}
				else if (su.StartsWith("EXPN"))
				{
					action = SmtpActionType.EXPN;
				}
				else if (su.StartsWith("VRFY"))
				{
					action = SmtpActionType.VRFY;
				}
				else if (su.StartsWith("HELP"))
				{
					action = SmtpActionType.HELP;
				}
				else
				{
					action = SmtpActionType.UNRECOG;
				}
			}

			SmtpRequest req = new SmtpRequest(action, requestParams, state);
			return req;
		}
	}
}
