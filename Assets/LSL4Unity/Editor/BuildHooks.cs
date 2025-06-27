using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;

namespace Assets.LSL4Unity.EditorExtensions
{
    public class BuildHooks
    {
        const string LIB_LSL_NAME = "liblsl";
        const string PLUGIN_DIR = "Plugins";

        [PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
            var buildHostDirectory = Path.GetDirectoryName(pathToBuiltProject);
            var dataDirectoryName = buildName + "_Data";
            var pathToDataDirectory = Path.Combine(buildHostDirectory, dataDirectoryName);
            var pluginDirectory = Path.Combine(pathToDataDirectory, PLUGIN_DIR);

            if (target == BuildTarget.StandaloneWindows)
            {
                RenameLibFile(pluginDirectory, LSLEditorIntegration.lib32Name, LSLEditorIntegration.lib64Name, LSLEditorIntegration.DLL_ENDING);
            }
            else if (target == BuildTarget.StandaloneWindows64)
            {
                RenameLibFile(pluginDirectory, LSLEditorIntegration.lib64Name, LSLEditorIntegration.lib32Name, LSLEditorIntegration.DLL_ENDING);
            }
            else if (target == BuildTarget.StandaloneLinux)
            {
                RenameLibFile(pluginDirectory, LSLEditorIntegration.lib32Name, LSLEditorIntegration.lib64Name, LSLEditorIntegration.SO_ENDING);
            }
            else if (target == BuildTarget.StandaloneLinux64)
            {
                RenameLibFile(pluginDirectory, LSLEditorIntegration.lib64Name, LSLEditorIntegration.lib32Name, LSLEditorIntegration.SO_ENDING);
            }
            else if (target == BuildTarget.StandaloneOSXIntel)
            {
                RenameLibFile(pluginDirectory, LSLEditorIntegration.lib32Name, LSLEditorIntegration.lib64Name, LSLEditorIntegration.BUNDLE_ENDING);
            }
            else if (target == BuildTarget.StandaloneOSXIntel64)
            {
                RenameLibFile(pluginDirectory, LSLEditorIntegration.lib64Name, LSLEditorIntegration.lib32Name, LSLEditorIntegration.BUNDLE_ENDING);
            }
        }

        private static void RenameLibFile(string pluginDirectory, string sourceName, string nameOfObsoleteFile, string fileEnding)
        {
            var obsoleteFile = Path.Combine(pluginDirectory, nameOfObsoleteFile + fileEnding);
            if (File.Exists(obsoleteFile))
            {
                Debug.Log("[LSL BUILD Hook] Deleting obsolete file: " + obsoleteFile);
                File.Delete(obsoleteFile);
            }
            else
            {
                Debug.Log("[LSL BUILD Hook] Obsolete file not found: " + obsoleteFile);
            }

            var sourceFile = Path.Combine(pluginDirectory, sourceName + fileEnding);
            var targetFile = Path.Combine(pluginDirectory, LIB_LSL_NAME + fileEnding);

            if (File.Exists(sourceFile))
            {
                Debug.Log(string.Format("[LSL BUILD Hook] Renaming: {0} → {1}", sourceFile, targetFile));
                File.Move(sourceFile, targetFile);
            }
            else
            {
                Debug.LogWarning("[LSL BUILD Hook] Source file not found: " + sourceFile + ". Possibly Unity renamed it (Unity 6 issue). Skipping rename.");
            }
        }
    }
}
