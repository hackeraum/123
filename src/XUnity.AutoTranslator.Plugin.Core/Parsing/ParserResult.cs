﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XUnity.AutoTranslator.Plugin.Core.Parsing
{
   internal class ParserResult
   {
      private static readonly string IgnoredNameEnding = "_i";

      public ParserResult( ParserResultOrigin origin, string originalText, string template, bool allowPartialTranslation, bool cacheCombinedResult, bool persistCombinedResult, bool persistTokenResult, Dictionary<string, string> args )
      {
         Origin = origin;
         OriginalText = originalText;
         TranslationTemplate = template;
         AllowPartialTranslation = allowPartialTranslation;
         CacheCombinedResult = cacheCombinedResult;
         PersistCombinedResult = persistCombinedResult;
         PersistTokenResult = persistTokenResult;
         Arguments = args;
      }

      public ParserResult( ParserResultOrigin origin, string originalText, string template, bool allowPartialTranslation, bool cacheCombinedResult, bool persistCombinedResult, bool persistTokenResult, Regex regex, Match match )
      {
         Origin = origin;
         OriginalText = originalText;
         TranslationTemplate = template;
         AllowPartialTranslation = allowPartialTranslation;
         CacheCombinedResult = cacheCombinedResult;
         PersistCombinedResult = persistCombinedResult;
         PersistTokenResult = persistTokenResult;
         Match = match;
         Regex = regex;
      }

      public ParserResultOrigin Origin { get; }
      public string OriginalText { get; }

      public string TranslationTemplate { get; }
      public Dictionary<string, string> Arguments { get; }
      public Match Match { get; }
      public Regex Regex { get; }

      public bool AllowPartialTranslation { get; }

      public bool CacheCombinedResult { get; }

      public bool PersistCombinedResult { get; }

      public bool PersistTokenResult { get; }

      public string GetTranslationFromParts( Func<string, string> getTranslation )
      {
         bool ok = true;
         var result = new StringBuilder( TranslationTemplate );
         if( Match != null )
         {
            var groups = Match.Groups;
            var groupNames = Regex.GetGroupNames();
            var len = groupNames.Length;
            for( int j = len - 1; j > 0; j-- )
            {
               var groupName = groupNames[ j ];
               var ignoreTranslate = groupName.EndsWith( IgnoredNameEnding );
               int.TryParse( groupName, NumberStyles.None, CultureInfo.InvariantCulture, out var groupIndex );

               Group group;
               string replacement;
               if( groupIndex != 0 )
               {
                  group = groups[ groupIndex ];
                  replacement = "$" + groupIndex;
               }
               else
               {
                  group = groups[ groupName ];
                  replacement = "${" + groupName + "}";
               }

               if( group.Success ) // was matched
               {
                  var value = group.Value;
                  var translation = ignoreTranslate ? value : getTranslation( value );
                  if( translation != null )
                  {
                     result = result.Replace( replacement, translation );
                  }
                  else
                  {
                     ok = false;
                  }
               }
               else
               {
                  result = result.Replace( replacement, string.Empty );
               }
            }
         }
         else
         {
            // This is really not a nice fix...
            if( Arguments.Count > 9 )
            {
               foreach( var kvp in Arguments.OrderByDescending( x => x.Key.Length ) )
               {
                  var translation = getTranslation( kvp.Value );
                  if( translation != null )
                  {
                     result = result.Replace( kvp.Key, translation );
                  }
                  else
                  {
                     ok = false;
                  }
               }
            }
            else
            {
               foreach( var kvp in Arguments )
               {
                  var translation = getTranslation( kvp.Value );
                  if( translation != null )
                  {
                     result = result.Replace( kvp.Key, translation );
                  }
                  else
                  {
                     ok = false;
                  }
               }
            }
         }

         if( ok )
         {
            return result.ToString();
         }
         return null;
      }
   }
}
