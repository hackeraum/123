﻿using System;
using System.IO;
using System.Linq;
using UnityEngine;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.Common.Constants;
using XUnity.Common.Logging;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core.Fonts
{
   internal static class FontHelper
   {
      public static UnityEngine.Object GetTextMeshProFont()
      {
         UnityEngine.Object font = null;

         var overrideFontPath = Path.Combine( Paths.GameRoot, Settings.OverrideFontTextMeshPro );
         if( File.Exists( overrideFontPath ) )
         {
            XuaLogger.AutoTranslator.Info( "Attempting to load TextMesh Pro font from asset bundle." );

            AssetBundle bundle = null;
            if( UnityTypes.AssetBundle_Methods.LoadFromFile != null )
            {
               bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.LoadFromFile.Invoke( null, new object[] { overrideFontPath } );
            }
            else if( UnityTypes.AssetBundle_Methods.CreateFromFile != null )
            {
               bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.CreateFromFile.Invoke( null, new object[] { overrideFontPath } );
            }
            else
            {
               XuaLogger.AutoTranslator.Error( "Could not find an appropriate asset bundle load method while loading font: " + overrideFontPath );
               return null;
            }

            if( bundle == null )
            {
               XuaLogger.AutoTranslator.Warn( "Could not load asset bundle while loading font: " + overrideFontPath );
               return null;
            }

            if( UnityTypes.FontAsset != null )
            {
               if( UnityTypes.AssetBundle_Methods.LoadAllAssets != null )
               {
#if MANAGED
                  var assets = (UnityEngine.Object[])UnityTypes.AssetBundle_Methods.LoadAllAssets.Invoke( bundle, new object[] { UnityTypes.FontAsset.UnityType } );
#else
                  var assets = (UnhollowerBaseLib.Il2CppReferenceArray<UnityEngine.Object>)UnityTypes.AssetBundle_Methods.LoadAllAssets.Invoke( bundle, new object[] { UnityTypes.FontAsset.UnityType } );
#endif
                  font = assets?.FirstOrDefault();
               }
               else if( UnityTypes.AssetBundle_Methods.LoadAll != null )
               {
#if MANAGED
                  var assets = (UnityEngine.Object[])UnityTypes.AssetBundle_Methods.LoadAll.Invoke( bundle, new object[] { UnityTypes.FontAsset.UnityType } );
#else
                  var assets = (UnhollowerBaseLib.Il2CppReferenceArray<UnityEngine.Object>)UnityTypes.AssetBundle_Methods.LoadAll.Invoke( bundle, new object[] { UnityTypes.FontAsset.UnityType } );
#endif
                  font = assets?.FirstOrDefault();
               }
            }
         }
         else
         {
            XuaLogger.AutoTranslator.Info( "Attempting to load TextMesh Pro font from internal Resources API." );

            font = Resources.Load( Settings.OverrideFontTextMeshPro );
         }

         if( font != null )
         {
            GameObject.DontDestroyOnLoad( font );
         }
         else
         {
            XuaLogger.AutoTranslator.Error( "Could not find the TextMeshPro font asset: " + Settings.OverrideFontTextMeshPro );
         }

         return font;
      }

      public static Font GetTextFont( int size )
      {
         var font = Font.CreateDynamicFontFromOSFont( Settings.OverrideFont, size );
         GameObject.DontDestroyOnLoad( font );

         return font;
      }

      public static string[] GetOSInstalledFontNames()
      {
         return Font.GetOSInstalledFontNames();
      }
   }
}
