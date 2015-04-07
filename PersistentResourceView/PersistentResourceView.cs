using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System;
namespace PersistentResourceView
{
	public class PersistentResourceViewDesc : IUserMod 
	{
		
		public string Name
		{
			get { return "Persistent Resource View"; }
		}
		
		public string Description 
		{
			get { return "Play mode resource view and all map editor views stay on until you turn it off or select another view"; }
		}
	}

	public class PersistentResourceView : ThreadingExtensionBase{
		public static InfoManager.InfoMode currentInfomode;

		public override void OnUpdate (float realTimeDelta, float simulationTimeDelta)
		{

			if(PersistentResourceViewLoader.toggleOn){
				if (!Singleton<InfoManager>.instance.CurrentMode.Equals(currentInfomode)){
					if (Singleton<InfoManager>.instance.CurrentMode.Equals(InfoManager.InfoMode.None)){
						Singleton<InfoManager>.instance.SetCurrentMode(currentInfomode, InfoManager.SubInfoMode.Default);
					}
					else{
						PersistentResourceViewLoader.toggleOn = false;
						currentInfomode = InfoManager.InfoMode.None;
					}
				}
				if (Input.GetKeyUp(KeyCode.Escape)){
					PersistentResourceViewLoader.toggleOn = false;
					currentInfomode = InfoManager.InfoMode.None;
					Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
				}

			} 

		}
	}

	public class PersistentResourceViewLoader : LoadingExtensionBase
	{
		public static bool toggleOn;
		public static LoadMode loadMode;
		
		public override void OnLevelLoaded(LoadMode mode)
		{
			toggleOn = false;
			loadMode = mode;
			PersistentResourceView.currentInfomode = InfoManager.InfoMode.None;
			if (mode.Equals(LoadMode.NewGame) || mode.Equals(LoadMode.LoadGame)){
				ColossalFramework.UI.UIButton resourceButton = GameObject.Find("InfoViewsPanel").transform.FindChild("Container").FindChild("Resources").gameObject.GetComponent<ColossalFramework.UI.UIButton>();
				resourceButton.eventClick += ButtonClick;
			}
			else if (mode.Equals(LoadMode.NewMap) || mode.Equals(LoadMode.LoadMap)){
				DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "mapstuff loaded");
				ColossalFramework.UI.UIButton[] infoviewButtons = GameObject.Find("InfoViewsPanel").transform.FindChild("Container").gameObject.GetComponentsInChildren<ColossalFramework.UI.UIButton>();
				foreach (ColossalFramework.UI.UIButton button in infoviewButtons){
					button.eventClick +=ButtonClick;
				}
			}
			
		}
		
		private void ButtonClick(UIComponent component, UIMouseEventParameter eventParam)
		{
			if (loadMode.Equals(LoadMode.NewGame) || loadMode.Equals(LoadMode.LoadGame)){
				if (component.name.Equals("Resources")){
					if (PersistentResourceView.currentInfomode.Equals(InfoManager.InfoMode.NaturalResources)){
						toggleOn = false;
						PersistentResourceView.currentInfomode = InfoManager.InfoMode.None;
					}
					else{
						PersistentResourceView.currentInfomode = InfoManager.InfoMode.NaturalResources;
						toggleOn = true;
					}

				}

			}
			else if (loadMode.Equals(LoadMode.NewMap) || loadMode.Equals(LoadMode.LoadMap)){
				if (PersistentResourceView.currentInfomode.Equals(getMode(component.name))){
					toggleOn = false;
					PersistentResourceView.currentInfomode = InfoManager.InfoMode.None;
				}
				else{
					PersistentResourceView.currentInfomode = getMode(component.name);
					toggleOn = true;

				}



			}
		}

		private InfoManager.InfoMode getMode(String buttonName){
			switch(buttonName){
			case "Water":
				return InfoManager.InfoMode.Water;
			case "Resources":
				return InfoManager.InfoMode.NaturalResources;
			case "WindSpeed":
				return InfoManager.InfoMode.Wind;
			case "PublicTransport":
				return InfoManager.InfoMode.Transport;
			case "TerrainHeight":
				return InfoManager.InfoMode.TerrainHeight;
			default:
				return InfoManager.InfoMode.None;
			}
		}
	}
}

