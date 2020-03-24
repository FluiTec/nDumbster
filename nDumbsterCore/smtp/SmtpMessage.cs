﻿#region Using directives

using System;
using System.Collections.Specialized;
using System.Text;

#endregion
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
	/// Container for a complete SMTP message - headers and message body.
	/// </summary>
	public class SmtpMessage
	{	
		/// <summary>
		/// Message body.
		/// </summary>
        // ReSharper disable once UnusedMember.Global
        public virtual string Body => _body.ToString();

        /// <summary>
        /// The raw SMTP message
        /// </summary>
        public virtual string RawMessage => _rawMessageBuilder.ToString();

        /// <summary>
		/// Number of lines in message body.
		/// </summary>
        // ReSharper disable once UnusedMember.Global
        public int BodyLineCount => _bodyLineCount;

        /// <summary>
		/// Headers: Collection of named values.
		/// </summary>
		private readonly NameValueCollection _headers; 

		/// <summary>
		/// Message body.
		/// </summary>
        private readonly StringBuilder _body;
        private readonly StringBuilder _rawMessageBuilder = new StringBuilder();

		/// <summary>
		/// Number of lines in body
		/// </summary>
		private int _bodyLineCount;

		/// <summary>
		/// Default carriage return code is CRLF.
		/// </summary>
		public const string DEFAULT_CRLF = "\r\n";

		/// <summary>
		/// Code used for carriage return in the body.
		/// </summary>
		public static string Cr = DEFAULT_CRLF;

		/// <summary>
		/// Constructor. Initializes headers collection and body buffer.
		/// </summary>
		public SmtpMessage()
		{
			_headers = new NameValueCollection();
			_body = new StringBuilder();
			_bodyLineCount = 0;
		}

	    /// <summary>
	    /// Determines whether or not this is a multi-part message
	    /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool IsMultiPartMessage => _headers["Content-Type"].IndexOf("multipart", StringComparison.InvariantCultureIgnoreCase) != -1;

        /// <summary> 
		/// Update the headers or body depending on the SmtpResponse object and line of input.
		/// </summary>
		/// <param name="response">SmtpResponse object</param>
		/// <param name="commandData">remainder of input line after SMTP command has been removed</param>
		internal void Store(SmtpResponse response, string commandData)
		{
		    if (commandData == null) return;

		    if (SmtpState.DATA_HDR == response.NextState)
		    {
                _rawMessageBuilder.AppendLine(commandData);

		        int headerNameEnd = commandData.IndexOf(":", StringComparison.Ordinal);
		        if (headerNameEnd >= 0)
		        {
		            string name = commandData.Substring(0, (headerNameEnd) - (0)).Trim();
		            string valueRenamed = commandData.Substring(headerNameEnd + 1).Trim();

		            // We use the Add method instead of [] because we can have multiple values for a name
		            _headers.Add(name, valueRenamed);
		        }
		    }
		    else if (SmtpState.DATA_BODY == response.NextState)
            {
                // The end of the headers is signified by a newline
                if (_body.Length == 0) _rawMessageBuilder.Append(Environment.NewLine);
                if (_bodyLineCount > 0) _rawMessageBuilder.Append(Cr);
                _rawMessageBuilder.Append(commandData);

		        if (_bodyLineCount > 0) _body.Append(Cr);
		        _body.Append(commandData);
		        _bodyLineCount++;

                //if (!IsMultiPartMessage) return;

                //Match boundaryMatch = Regex.Match(body.ToString(), @"boundary=""?(?<boundary>[^"";\r\n]+)""?;?", RegexOptions.IgnoreCase);
                //_BaseBoundary = boundaryMatch.Groups["boundary"].Value;
		    }
		}

	    /// <summary>
		/// Headers of the message.
		/// </summary>
        // ReSharper disable once UnusedMember.Global
        public NameValueCollection Headers => _headers;

        /// <summary>
		/// String representation of the SmtpMessage.
		/// </summary>
		/// <returns>A String that displays the current SmtpMessage headers and body</returns>
		public override string ToString()
		{
			StringBuilder msg = new StringBuilder();
			foreach (string name in _headers.AllKeys)
			{
                // ReSharper disable once PossibleNullReferenceException
                foreach(string val in _headers.GetValues(name))
				{
					msg.Append($"{name}: {val}\n");					
				}
			}

			msg.Append('\n');
			msg.Append(_body);
			msg.Append('\n');
			return msg.ToString();
		}
	}
}
