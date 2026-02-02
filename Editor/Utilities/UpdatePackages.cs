using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace BioluminescentGames.Utils.Editor.Utilities
{
    // TODO: Update git packages
    public static class UpdatePackages
    {
        [MenuItem("Tools/Bioluminescent Games/Update Packages")]
        private static async void PerformUpdate()
        {
            try
            {
                ListRequest listRequest = Client.List();

                while (!listRequest.IsCompleted)
                    await Task.Yield();

                if (listRequest.Status != StatusCode.Success)
                {
                    Debug.LogError($"Error, Code: {listRequest.Error.errorCode}, Message: {listRequest.Error.message}");
                    return;
                }

                PackageCollection packages = listRequest.Result;

                List<string> packagesToUpdate = new();
                foreach (PackageInfo package in packages)
                {
                    Debug.Log($"Package Type: {package.type}");

                    if (string.IsNullOrWhiteSpace(package.versions.recommended))
                    {
                        //Debug.Log($"Package GitHash: {package.git.hash}");
                        if (string.IsNullOrWhiteSpace(package.git.hash))
                            continue;

                        //Debug.Log($"Package Repo URL: {package.repository.url}, Repo Type: {package.repository.type}");

                        packagesToUpdate.Add(package.name); // no idea if this works

                        continue;
                    }

                    if (package.versions.recommended == package.version)
                        continue;

                    Debug.Log($"Updating {package.displayName} ({package.name}) from {package.version} to {package.versions.recommended}");
                    packagesToUpdate.Add($"{package.name}@{package.versions.recommended}");
                }

                AddAndRemoveRequest updateRequest = Client.AddAndRemove(packagesToUpdate.ToArray());

                while (!updateRequest.IsCompleted)
                    await Task.Yield();

                if (updateRequest.Status != StatusCode.Success)
                {
                    Debug.LogError($"Error, Code: {updateRequest.Error.errorCode}, Message: {updateRequest.Error.message}");
                    return;
                }

                Debug.Log("Successfully updated packages.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
