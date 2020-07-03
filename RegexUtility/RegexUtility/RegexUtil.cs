using System;
using System.Text;
using Crestron.SimplSharp;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace RegexUtility
{
    public class RegexUtil
    {
        #region Properties

        private List<string> _strings = new List<string>();
        //private string _regex { get; set; } //"\"(.*?)\""
        public static ushort _size { get; private set; }

        
        public delegate void ReturnSingleMatchEventHandler(SimplSharpString match);
        public ReturnSingleMatchEventHandler ReturnSingleMatchDelegate { get; set; }

        public delegate void ReturnMatchesEventHandler(object sender, Matches matches);
        public event ReturnMatchesEventHandler ReturnMatchesEvent;

        #endregion

        #region Public Methods
        /// <summary>
        /// Load strings to test against _regex pattern
        /// </summary>
        /// <param name="str"></param>
        public void LoadStrings(string str)
        {
            _strings.Add(str);
        }

        /// <summary>
        /// Test a single string against the regex pattern
        /// </summary>
        /// <param name="str">String to be tested against regex pattern</param>
        /// <param name="regex">pattern to test</param>
        /// <returns>1 if match is found, 0 if match is not found</returns>
        public ushort TestMatch(string str, string regex)
        {
            Match match;
            match = Regex.Match(str, regex);
            ReturnSingleMatchDelegate(match.Value);
            if (match.Success) 
                return 1;
            return 0;
        }

        /// <summary>
        /// Test a group of strings loaded from S+ to S# against the _regex pattern
        /// returns an event object containing all of the matches
        /// </summary>
        public void GroupMatch(string pattern, ushort size)
        {
            _size = size;
            Match match;
            var args = new Matches();

            foreach (var str in _strings)
            {
                match = Regex.Match(str, pattern);
                if (match.Success)
                {
                    args.MatchList[args.MatchCount++] = match.Groups[1].Value;
                }
            }
            _strings.Clear();

            if (ReturnMatchesEvent != null)
            {
                ReturnMatchesEvent(this, args);
            }
        }

        /// <summary>
        /// Find all matches given a single string
        /// </summary>
        /// <param name="str"></param>
        public void FindAllMatches(string str, string pattern, ushort size)
        {
            _size = size;
            MatchCollection matchCollection;
            var args = new Matches();

            matchCollection = Regex.Matches(str, pattern);
            foreach (Match matchItem in matchCollection)
            {
                args.MatchList[args.MatchCount++] = matchItem.Value;
            }

            if (ReturnMatchesEvent != null)
            {
                ReturnMatchesEvent(this, args);
            }
        }

        /// <summary>
        /// Replace string that matches the regex pattern with the replacement string
        /// </summary>
        /// <param name="str">string to search for match</param>
        /// <param name="pattern">regex pattern to match</param>
        /// <param name="replacement">replacement string</param>
        /// <returns></returns>
        public string Replace(string str, string pattern, string replacement)
        {
            return Regex.Replace(str, pattern, replacement);
        }

        /// <summary>
        /// splits an input string into an array of substring given a pattern
        /// </summary>
        /// <param name="str">string to split</param>
        /// <param name="pattern">regex pattern to match</param>
        public void Split(string str, string pattern, ushort size)
        {
            _size = size;
            var args = new Matches();
            string[] results = pattern.Equals(".") ? str.Split(pattern[0]) : Regex.Split(str, pattern);

            foreach (var result in results)
            {
                args.MatchList[args.MatchCount++] = result;
            }
            if (ReturnMatchesEvent != null)
            {
                ReturnMatchesEvent(this, args);
            }
        }

        #endregion
    }

    /// <summary>
    /// Matches class contains a string array containing all the matches to be returned to s+
    /// </summary>
    public class Matches
    {
        public string[] MatchList = new string[RegexUtil._size];
        public uint MatchCount = 0;
    }
}
