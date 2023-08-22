using Aki.Reflection.Patching;
using EFT;
using System.Reflection;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestZoneAPI.Patches
{
    public class GameWorldPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
        }

        public static List<ZoneClass> GetZones()
        {
            Logger.LogInfo("Getting Custom Quest Zones...");
            var zones = Helpers.WebRequestHelper.Get<List<ZoneClass>>("/quests/zones/getZones");
            Logger.LogInfo(zones.First().ZoneName);
            return zones;
        }

        public static void CreatePlaceItemZone(ZoneClass zone)
        {
            Logger.LogInfo("Creating Zone...");
            Logger.LogInfo($"{zone.ZoneName}");
            GameObject questZone = new GameObject();

            BoxCollider collider = questZone.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            Vector3 position = new Vector3(float.Parse(zone.Position.X), float.Parse(zone.Position.Y), float.Parse(zone.Position.Z));
            Vector3 scale = new Vector3(float.Parse(zone.Scale.X), float.Parse(zone.Scale.Y), float.Parse(zone.Scale.Z));
            questZone.transform.position = position;
            questZone.transform.localScale = scale;
            EFT.Interactive.PlaceItemTrigger scriptComp = questZone.AddComponent<EFT.Interactive.PlaceItemTrigger>();
            scriptComp.SetId(zone.ZoneId);

            questZone.layer = LayerMask.NameToLayer("Triggers");
            questZone.name = zone.ZoneId;
        }

        public static void CreateVisitZone(ZoneClass zone)
        {
            Logger.LogInfo("Creating Zone...");
            Logger.LogInfo(zone.ZoneName);
            GameObject questZone = new GameObject();

            BoxCollider collider = questZone.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            Vector3 position = new Vector3(float.Parse(zone.Position.X), float.Parse(zone.Position.Y), float.Parse(zone.Position.Z));
            Vector3 scale = new Vector3(float.Parse(zone.Scale.X), float.Parse(zone.Scale.Y), float.Parse(zone.Scale.Z));
            questZone.transform.position = position;
            questZone.transform.localScale = scale;
            EFT.Interactive.ExperienceTrigger scriptComp = questZone.AddComponent<EFT.Interactive.ExperienceTrigger>();
            scriptComp.SetId(zone.ZoneId);

            questZone.layer = LayerMask.NameToLayer("Triggers");
            questZone.name = zone.ZoneId;
        }

        public static void AddZones(List<ZoneClass> zones, string currentLocation)
        {
            Logger.LogInfo("Adding all zones...");
            foreach (ZoneClass zone in zones)
            {
                if (zone.ZoneLocation.ToLower() == currentLocation.ToLower())
                {
                    switch(Enum.Parse(typeof(ZoneType), zone.ZoneType))
                    {
                        case ZoneType.PlaceItem:
                            Logger.LogInfo(zone.Position.X);
                            Logger.LogInfo(zone.Rotation.Y);
                            CreatePlaceItemZone(zone);

                            break;
                        case ZoneType.Visit:
                            Logger.LogInfo(zone.Position.X);
                            Logger.LogInfo(zone.Rotation.Y);
                            CreateVisitZone(zone);
                            break;
                        default:
                            Logger.LogInfo(zone.Position.X);
                            Logger.LogInfo(zone.Rotation.Y);
                            CreateVisitZone(zone);
                            break;
                    }
                } else
                {
                    Logger.LogInfo("Zone not in current location");
                }
            }
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            Logger.LogInfo("GameWorld.OnGameStarted [QuestZoneAPI]");
            try
            {
                string loc = __instance.MainPlayer.Location;
                Logger.LogInfo(loc); // works
                
                List<ZoneClass> zones = GetZones();
                AddZones(zones, loc);
            } catch (Exception ex)
            {
                Logger.LogInfo("GameWorld Error: " + ex.Message);
            }
        }
    }
}
