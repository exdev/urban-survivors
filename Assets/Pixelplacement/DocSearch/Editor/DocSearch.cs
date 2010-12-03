//by Bob Berkebile : Pixel Placement : http://www.pixelplacement.com
//Version 1.0.1

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class DocSearch : EditorWindow {
	static GUIStyle bevelLineStyle;
	static bool initialized;
	Vector2 termsScroller, mainScroll;
	List<string> searchedTerms = new List<string>();
	int termID = -1;
	string searchTerm = "";
	bool useLocal, showRecent = false;
	string localScriptDocURL = "/Documentation/Documentation/ScriptReference/30_search.html?q=";
	string webScriptDocURL = "http://unity3d.com/support/documentation/ScriptReference/30_search.html?q=";
	string historyFile = "/Pixelplacement/DocSearch/Editor/history.txt";
	
	[MenuItem("Pixelplacement/Doc Search")]
	public static void Init(){
		GetWindow(typeof(DocSearch), false, "Doc Search");	
	}
	
	void OnGUI(){
		mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
		LocalToggle();
		SearchField();
		RecentTerms();
		EditorGUILayout.EndScrollView();
	}
	
	void OnEnable(){	
		if(!initialized){
			initialized=true;
			bevelLineStyle = new GUIStyle();
			bevelLineStyle.normal.background = (Texture2D)Resources.Load("_bevelLine");
			//
			if(File.Exists(Application.dataPath + historyFile)){
				StreamReader sr = new StreamReader(Application.dataPath + historyFile);
				string history = sr.ReadToEnd();
				string[] terms = history.Split(',');
				searchedTerms= new List<string>();
				for (int i = 0; i < terms.Length; i++) {
					searchedTerms.Add(terms[i]);
				}
				sr.Close();
			}
		}
	}
	
	void LocalToggle(){
		EditorGUILayout.Space();
		if(Application.platform != RuntimePlatform.WindowsEditor){
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Use Local Documentation:");
			useLocal = EditorGUILayout.Toggle(useLocal);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
	
	void SearchField(){
		Event e = Event.current;
		EditorGUILayout.BeginHorizontal();
		GUI.SetNextControlName("searchField");
		searchTerm = EditorGUILayout.TextField(searchTerm);
		GUI.SetNextControlName("searchButton");
		if(GUILayout.Button("Search")){
			GUI.FocusControl("searchButton");
			PerformSearch(searchTerm);
			searchTerm = "";
		}
		EditorGUILayout.EndHorizontal();
		
		if(e.isKey){
			if(e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter){
				GUI.FocusControl("searchButton");
				PerformSearch(searchTerm);
				searchTerm = "";	
				e.Use();
			}			
		}
	}
	
	void RecentTerms(){
		if(searchedTerms.Count > 0){
			GUILayout.Button("",bevelLineStyle);
			showRecent = EditorGUILayout.Foldout(showRecent,"Recent Searches");	
			if(showRecent){
				termsScroller = EditorGUILayout.BeginScrollView(termsScroller);
				termID = GUILayout.SelectionGrid(termID,searchedTerms.ToArray(),1);
				EditorGUILayout.EndScrollView();
				GUILayout.Button("",bevelLineStyle);
				if(GUILayout.Button("Clear Recent Searches")){
					searchedTerms = new List<string>();
					File.Delete(Application.dataPath + historyFile);
					AssetDatabase.Refresh();
				}
				EditorGUILayout.Space();
				if(termID != -1){
					string term = searchedTerms.ToArray()[termID];
					searchedTerms.RemoveAt(termID);
					PerformSearch(term);
					termID = -1;
				}
			}
		}	
	}
	
	void PerformSearch(string term){
		if(term != ""){
			term = term.ToLower();
			string searchPath;
			if(useLocal){
				string pathPrefix;
				if(Application.platform == RuntimePlatform.WindowsEditor){
					pathPrefix = "file:///";
				}else{
					pathPrefix = "file://";
				}
				searchPath = pathPrefix + EditorApplication.applicationContentsPath + localScriptDocURL;
				string[] pathPieces = searchPath.Split(' ');
				searchPath = String.Join("%20", pathPieces);
			}else{
				searchPath = webScriptDocURL;
			}
			Application.OpenURL(searchPath + WWW.EscapeURL(term));		
			
			if(searchedTerms.IndexOf(term) == -1){
				searchedTerms.Reverse();
				searchedTerms.Add(term);
				searchedTerms.Reverse();
			}else{
				searchedTerms.RemoveAt(searchedTerms.IndexOf(term));
				searchedTerms.Reverse();
				searchedTerms.Add(term);
				searchedTerms.Reverse();
			}
			
			CatalogSearches();
		}		
	}
	
	void CatalogSearches(){
		StreamWriter sw = new StreamWriter(Application.dataPath + historyFile,false);
		for (int i = 0; i < searchedTerms.Count; i++) {
			string entry;
			if(i < searchedTerms.Count-1){
				entry = searchedTerms[i] + ",";
			}else{
				entry = searchedTerms[i];
			}
			sw.Write(entry);
		}
		sw.Close();
		AssetDatabase.Refresh();
	}
}