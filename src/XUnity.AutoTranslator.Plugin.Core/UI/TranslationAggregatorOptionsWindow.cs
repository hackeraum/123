﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XUnity.AutoTranslator.Plugin.Core.UI
{
   internal class TranslationAggregatorOptionsWindow
   {
      private const int WindowId = 45733721;
      private const float WindowWidth = 300;

      private Rect _windowRect = new Rect( 20, 20, WindowWidth, 400 );
      private bool _isMouseDownOnWindow = false;
      private TranslationAggregatorViewModel _viewModel;
      private List<ToggleViewModel> _toggles;
      private Vector2 _scrollPosition;

      public TranslationAggregatorOptionsWindow( TranslationAggregatorViewModel viewModel )
      {
         _viewModel = viewModel;
         _toggles = _viewModel.Endpoints.Select( x =>
         new ToggleViewModel(
            x.Endpoint.Endpoint.FriendlyName,
            null,
            null,
            () => x.IsEnabled = !x.IsEnabled,
            () => x.IsEnabled ) ).ToList();
      }

      public bool IsShown
      {
         get => _viewModel.IsShowingOptions;
         set => _viewModel.IsShowingOptions = value;
      }

      public void OnGUI()
      {
         GUI.Box( _windowRect, GUIContent.none, GUIUtil.GetWindowBackgroundStyle() );

         _windowRect = GUI.Window( WindowId, _windowRect, CreateWindowUI, "---- Translation Aggregator Options ----" );

         if( GUIUtil.IsAnyMouseButtonOrScrollWheelDown )
         {
            var point = new Vector2( Input.mousePosition.x, Screen.height - Input.mousePosition.y );
            _isMouseDownOnWindow = _windowRect.Contains( point );
         }

         if( !_isMouseDownOnWindow || !GUIUtil.IsAnyMouseButtonOrScrollWheel )
            return;

         // make sure window is focused if scroll wheel is used to indicate we consumed that event
         GUI.FocusWindow( WindowId );

         var point1 = new Vector2( Input.mousePosition.x, Screen.height - Input.mousePosition.y );
         if( !_windowRect.Contains( point1 ) )
            return;

         Input.ResetInputAxes();
      }

      private void CreateWindowUI( int id )
      {
         if( GUI.Button( GUIUtil.R( WindowWidth - 22, 2, 20, 16 ), "X" ) )
         {
            IsShown = false;
         }

         GUILayout.Label( "Available Translators" );

         // GROUP
         _scrollPosition = GUILayout.BeginScrollView( _scrollPosition, GUI.skin.box );
         
         foreach( var vm in _toggles )
         {
            var previousValue = vm.IsToggled();
            var newValue = GUILayout.Toggle( previousValue, vm.Text );
            if( previousValue != newValue )
            {
               vm.OnToggled();
            }
         }

         GUILayout.EndScrollView();

         GUILayout.Label( "Height per Translator" );

         _viewModel.HeightPerTranslator = GUILayout.HorizontalSlider( _viewModel.HeightPerTranslator, 50, 300 );

         GUI.DragWindow();

      }
   }
}
