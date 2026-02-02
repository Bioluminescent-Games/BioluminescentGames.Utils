using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace BioluminescentGames.Utils.Editor.Utilities
{
    public static class UpdatePackages
    {
        [MenuItem("Tools/Bioluminescent Games/Update All Git Packages")]
        private static async void UpdateGitPackages()
        {
            const string title = "Updating Git Packages";

            ListRequest listRequest = Client.List();

            while (!listRequest.IsCompleted)
                await Task.Yield();

            if (listRequest.Status != StatusCode.Success)
            {
                string error = $"Error listing packages: {listRequest.Error.errorCode} - {listRequest.Error.message}";
                Debug.LogError(error);
                EditorUtility.DisplayDialog(title, error, "Ok");
                return;
            }

            //int currentGitCount = 0;

            PackageInfo[] gitPackageList = listRequest.Result
                .Where(x => x.source == PackageSource.Git).ToArray();
            /*Dictionary<string, PackageInfo> gitPackages = gitPackageList
                .ToDictionary(GitUriFromPackage, x => x);*/

            AddAndRemoveRequest request = Client.AddAndRemove(gitPackageList.Select(x => x.packageId).ToArray());

            while (!request.IsCompleted)
                await Task.Yield();

            if (request.Status != StatusCode.Success)
            {
                string error = $"Error updating packages: {request.Error.errorCode} - {request.Error.message}";
                Debug.LogError(error);
                EditorUtility.DisplayDialog(title, error, "Ok");
                return;
            }

            /*IOrderedEnumerable<KeyValuePair<string, PackageInfo>> orderedPackages = gitPackages.OrderBy(x =>
            {
                PackageInfo packageInfo = x.Value;
                return packageInfo.dependencies.Sum(dep => gitPackageList.Count(p => p.name == dep.name));
            });

            foreach ((string gitUri, PackageInfo package) in orderedPackages)
            {
                string fullPackageId = package.packageId;

                Client.AddAndRemove()
            }*/

            return;

            //float GetProgress() => currentGitCount / (float) gitPackages.Count;

            // packageId is formatted as "com.author.package@git@github.com:author/package.git"
            /*string GitUriFromPackage(PackageInfo package)
            {
                try
                {
                    return package.packageId.Split('@', 2)[1];
                }
                catch (IndexOutOfRangeException) // future-proofing?
                {
                    return package.packageId;
                }
            }*/
        }
    }
}
