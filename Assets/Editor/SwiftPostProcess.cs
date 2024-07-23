using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class SwiftPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Path to the Xcode project
            var projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

            // Get target GUIDs
            var targetGuid = proj.GetUnityMainTargetGuid();
            var frameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();

            // Disable Bitcode
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            // Set Swift bridging header and interface header
            proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/UnityPlugin-Bridging-Header.h");
            proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "UnityIosPlugin-Swift.h");

            // Update build properties
            proj.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
            proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
            proj.AddBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.AddBuildProperty(targetGuid, "DYLIB_INSTALL_NAME_BASE", "@rpath");
            proj.AddBuildProperty(targetGuid, "LD_DYLIB_INSTALL_NAME", "@executable_path/../Frameworks/$(EXECUTABLE_PATH)");
            proj.AddBuildProperty(targetGuid, "DEFINES_MODULE", "YES");
            proj.AddBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
            proj.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");

            // Write updated properties to the project file
            proj.WriteToFile(projPath);

            // Process Info.plist
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var rootDict = plist.root;
            rootDict.SetBoolean("UIFileSharingEnabled", true);
            rootDict.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);

            File.WriteAllText(plistPath, plist.WriteToString());

            // Ensure NativeShare files are included
            var nativeSharePath = Path.Combine(buildPath, "Libraries/Plugins/iOS/NativeShare.m");
            if (File.Exists(nativeSharePath))
            {
                proj.AddFileToBuild(frameworkTargetGuid, proj.AddFile(nativeSharePath, nativeSharePath));
            }

            var nativeShareHeaderPath = Path.Combine(buildPath, "Libraries/Plugins/iOS/NativeShare.h");
            if (File.Exists(nativeShareHeaderPath))
            {
                proj.AddFileToBuild(frameworkTargetGuid, proj.AddFile(nativeShareHeaderPath, nativeShareHeaderPath));
            }

            var nativeShareBridgePath = Path.Combine(buildPath, "Libraries/Plugins/iOS/NativeShareBridge.m");
            if (File.Exists(nativeShareBridgePath))
            {
                proj.AddFileToBuild(frameworkTargetGuid, proj.AddFile(nativeShareBridgePath, nativeShareBridgePath));
            }
        }
    }
}
