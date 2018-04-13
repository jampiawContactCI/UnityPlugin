﻿using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Leap.Unity {

  [CustomEditor(typeof(PostProcessProvider), editorForChildClasses: true)]
  public class PostProcessProviderEditor : CustomEditorBase<PostProcessProvider> {

    protected override void OnEnable() {
      base.OnEnable();

      // Edit-time pose is only relevant for providers that generate hands.
      // Post-process Providers are a special case and don't generate their own hands.
      specifyConditionalDrawing(() => false, "editTimePose");

      specifyCustomDecorator("_inputLeapProvider", decorateInputLeapProvider);
    }

    private void decorateInputLeapProvider(SerializedProperty property) {
      var provider = this.target;

      if (!provider.enabled) {
        EditorGUILayout.HelpBox(
          message: "This post-process provider is disabled, so it will not output any "
                 + "hand data. Use pass-through mode if you only want to disable its "
                 + "post-processing and still output hands.",
          type: MessageType.Info
        );
      }
    }

  }

}
