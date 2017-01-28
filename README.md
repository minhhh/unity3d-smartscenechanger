# Smart Scene Changer (Unity3d version 5.5.0 or higher)

----
## What is [Smart Scene Changer] ?
[Smart Scene Changer] is script assets to change scene with showing now loading screen and load any startup contents like WWW or AssetBundle in background. 

----
## Asset Store
https://www.assetstore.unity3d.com/jp/#!/content/80061

----
## Features
* Scene change with now loading screen
* Scene change with AssetBundle loading in background
* Scene change with WWW loading in background
* Scene change with IEnumerator in background
* Parallel downloadings
* Show error dialog if an error occurred
* Solve AssetBundle dependencies automatically by manifest
* Solve additive scene in AssetBundle
* Decryption of AssetBundle
* You can override now loading contents
* You can override error dialog contents

----
## How to start demo scene

### Prepare a server (or use StreamingAssets folder)
Prepare a server for WWW or AssetBundle download. If you don't have any server, there is a server software I created, use below.  
[https://bitbucket.org/ciitt/simple-send-server](https://bitbucket.org/ciitt/simple-send-server)

### Build AssetBundles
If you don't have any build script, use below.  
Tools -> SSC -> Sample Build AssetBundles Window  
(you can override [BuildAssetBundlesWindow] script)  
(I created this tool for any tests)

### Copy AssetBundle files to your server or StreamingAssets folder.

### Set AssetBundle Manifest File Url
Open [SSC Demo Init Scene] and select [SmartSceneChangerSample] object.  
Find [SampleAssetBundleStartupManager] script and set parameter values below.

* Encrypted Manifest File Url (in case of [Use Decryption] is true)
* Manifest File Url (in case of [Use Decryption] is false)

(or override script as you want)  
(if you changed [Use Decryption], clear cache.)

### Activate sample WWW loading script
Open [SSC Sample Scene] and select [sample www startup] object.  
Find [SampleWwwStartupScript] script and set [Url] parameter value (url to image file).  
Activate this script.

### Check Scenes
* Open [SSC Demo Init Scene] and check [SmartSceneChangerSample] object.  
This is a singleton prefab for scene changing.

* Open [SSC Sample Scene] and check objects that have a name "sample".  
These are startup scripts of the scene.
    
* Open [Additive Scene 01] and [Additive Scene 02].  
These are the additive scenes from AssetBundle.

* Open [SSC Dummy Title].  
This scene will be loaded when scene change failed.

### Start
Open [SSC Demo Init Scene] and play.  
[SSC Sample Scene] will be loaded with any loadings.

----
## How to use and implement startup scripts

### How to change scene

* Code
SSC.SceneChangeManager.Instance.loadNextScene("scene_name");

* Sample scene, object, script  
SSC Demo Init Scene -> Start -> StartTestScript

### Now loading contents

* Override [NowLoadingBaseScript], and implement abstarct functions.  
And put your object to [SceneChangeManager].

* Sample scene, object, script  
SSC Demo Init Scene -> NowLoading Panel Standard -> SampleNowLoadingStandardScript  
SSC Demo Init Scene -> NowLoading Panel Stencil -> SampleNowLoadingStencilScript

### YesNo dialog contents

Yesno dialog is used for showing error message from [Smart Scene Changer].  
If yes clicked, restart from where it failed.  
If no clicked, show ok dalog and back to title scene.

* Override [YesNoDialogBaseScript], and implement abstarct functions.  
And put your object to [DialogManager].

* Sample scene, object, script  
SSC Demo Init Scene -> Yesno Dialog -> SampleYesNoDialogScript

### Ok dialog contents

Ok dialog is called from YesNo dialog.

* Override [OkDialogBaseScript], and implement abstarct functions.  
And put your object to [DialogManager].

* Sample scene, object, script  
SSC Demo Init Scene -> Ok Dialog -> SampleOkDialogScript

### Loading AssetBundles at start of scene

* Override [AssetBundleStartupScript], and implement abstarct functions.  
Overrided script is added to [AssetBundleStartupManager] automatically.

* Sample scene, object, script  
SSC Sample Scene -> sample ab scene startup -> SampleAssetBundleStartupScript  
SSC Sample Scene -> sample ab startup -> SampleAssetBundleStartupScript

### Loading WWW at start of scene

* Override [WwwStartupScript], and implement abstarct functions.  
Overrided script is added to [WwwStartupManager] automatically.

* Sample scene, object, script  
SSC Sample Scene -> sample www startup -> SampleAssetBundleStartupScript

### Start IEnumerator at start of scene

* Override [IEnumeratorStartupScript], and implement abstarct functions.  
Overrided script is added to [IEnumeratorStartupManager] automatically.

* Sample scene, object, script  
SSC Sample Scene -> sample ie startup -> SampleIEnumeratorStartupScript

### Implement how to deal with AssetBundle manifest url, AssetBundle url, and decrypting AssetBundle.

* Override [AssetBundleStartupManager], and implement virtual functions.

* Sample scene, object, script  
SSC Demo Init Scene -> SmartSceneChangerSample -> SampleAssetBundleStartupManager

### Implement how to choose now loading object

* Override [SceneChangeManager], and implement virtual functions.

* Sample scene, object, script  
SSC Demo Init Scene -> SmartSceneChangerSample -> SampleSceneChangeManager

### Create a manager prefab

Copy [SmartSceneChangerBase] prefab and implement the prefab with referring to [SmartSceneChangerSample] prefab.
    Create first scene and put the prefab into the scene.

### Create scenes

Create scenes with referring to [SSC Sample Scene].
    Startup scripts are automatically added to each manager at start of the scene.

----
## Flow of scene change

* Start intro of now loading
* Start main loop of now loading
* Start [UnloadUnusedAssets]
* Start [LoadSceneAsync]
* Start loading all of [IEnumeratorStartupScript(before)]
* Start loading all of [AssetBundleStartupScript]
* Start loading all of [WwwStartupScript]
* Start loading all of [IEnumeratorStartupScript(after)]
* Wait for main loop of now loading has finished
* Start outro of now loading

----
## Tools

* Tools -> SSC -> Editor Scene Loader Window  
Quick scene access tool in editor.

* Tools -> SSC -> Sample Build AssetBundles Window  
Build AssetBundles tool with encryption.

----
## Warnings

* [SmartSceneChangerBase] prefab has [EventSystem], so be careful not to be duplicated.

----
## License

MIT