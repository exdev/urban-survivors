// ======================================================================================
// File         : MissionLevelEditor.cs
// Author       : Wu Jie 
// Last Change  : 02/18/2011 | 16:49:41 PM | Friday,February
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class MissionLevelEditor 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[CustomEditor(typeof(MissionLevel))]
class MissionLevelEditor : Editor {

    string editTemplateName = "levelup template";
    int editMaxAliveUp = 1;
    float editHpUp = 1.0f;
    float editAttackUp = 1.0f;

	GUIStyle style = new GUIStyle();

    GameObject[] applyTargets = new GameObject[0];

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void OnEnable () {

		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.blue;
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {		
        MissionLevel editTarget = target as MissionLevel; 

        TemplateEditGUI();
		EditorGUILayout.Space();

        //
        GUILayout.Label( " Edit levels:", style );
        int levelCount = EditorGUILayout.IntField("Num of levels: ", editTarget.levels.Length );
        if ( levelCount != editTarget.levels.Length ) {

            MissionLevel.LevelUpBehavior[] newLevels = new MissionLevel.LevelUpBehavior[levelCount]; 
            for ( int i = 0; i < levelCount; ++i ) {
                newLevels[i] = new MissionLevel.LevelUpBehavior();
            }

            for ( int i = 0; i < editTarget.levels.Length; ++i ) {
                if ( levelCount > i )
                    newLevels[i] = editTarget.levels[i];
            }
            editTarget.levels = newLevels;
        }

        //
        for ( int i = 0; i < levelCount; ++i ) {
            EditorGUILayout.TextField( "level " + i );
            // editTarget.levels[i].name = 
        }

        //update and redraw:
        if ( GUI.changed ) {
            EditorUtility.SetDirty(editTarget);			
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void TemplateEditGUI () {
        MissionLevel editTarget = target as MissionLevel; 

        // handle template 
        GUILayout.Label( " Edit new levelup template:", style );

        this.editTemplateName = EditorGUILayout.TextField( "name ", this.editTemplateName );
        this.editMaxAliveUp = EditorGUILayout.IntField( "max alive up ", this.editMaxAliveUp );
        this.editHpUp = EditorGUILayout.FloatField( "hp up ", this.editHpUp );
        this.editAttackUp = EditorGUILayout.FloatField( "max attack up ", this.editAttackUp );

        int targetCount = EditorGUILayout.IntField("Num Of Targets: ", this.applyTargets.Length );
        if ( targetCount != this.applyTargets.Length ) {
            GameObject[] newTargets = new GameObject[targetCount]; 

            for ( int i = 0; i < this.applyTargets.Length; ++i ) {
                if ( targetCount > i )
                    newTargets[i] = this.applyTargets[i];
            }
            this.applyTargets = newTargets;
        }
        for ( int i = 0; i < targetCount; ++i ) {
            EditorGUILayout.ObjectField( i.ToString(), this.applyTargets[i], typeof(GameObject) );
        }

        //
        if ( GUILayout.Button("Add") ) {
            MissionLevel.LevelUpTemplate new_tmpl = new MissionLevel.LevelUpTemplate();
            new_tmpl.name = this.editTemplateName;
            new_tmpl.targets = this.applyTargets;
            new_tmpl.max_alive_up = this.editMaxAliveUp;
            new_tmpl.hp_up = this.editHpUp;
            new_tmpl.attack_up = this.editAttackUp;

            //
            bool added = false;
            // foreach ( MissionLevel.LevelUpTemplate tmpl in editTarget.templates ) {
            for ( int i = 0; i < editTarget.templates.Count; ++i ) {
                if ( new_tmpl.name == editTarget.templates[i].name ) {
                    editTarget.templates[i] = new_tmpl;
                    added = true;
                    break;
                }
            }
            if ( !added )
                editTarget.templates.Add(new_tmpl);
        }

        // GUILayout.BeginHorizontal();  
        // GUILayout.EndHorizontal ();
    }
}
