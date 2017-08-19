(For detailed information, please see the link below)
https://bitbucket.org/ciitt/unity3d-smartscenechanger

# Smart Scene Changer

----
## What is [Smart Scene Changer] ?
[Smart Scene Changer] is script assets to change scene with showing now loading screen and load any startup contents like WWW or AssetBundle in background.

----
## Asset Store
https://www.assetstore.unity3d.com/jp/#!/content/80061

----
## Youtube
https://youtu.be/a0S9U4syaYw

----
## Features (ver2)
* (ver2 is NOT compatible with ver1)
* (Code refactoring)
* (Add MIT license text)
* New dialog manager
* Common UI manager
* Scene UI manager
* Loading AssetBundle in runtime
* Controling pause state
* Fix progress bug
* Fix SSC.SimpleReduManager bug
* Fix BuildAssetBundlesWindowPrefs editor reference bug

----
## Features (ver1, ver2)
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

* Ios Manifest File Url (for iOS)
* Android Manifest File Url (for Android)
* Win Manifest File Url (for Windows)

(or override AssetBundleStartupManager script as you want)  

### Activate sample WWW loading script
Open [SSC Sample Scene] and select [sample www startup] object.  
Find [SampleWwwStartupScript] script and set [Url] parameter value (url to image file).  
Activate this script.  

### Start
Open [SSC Demo Init Scene] and play.  
[SSC Sample Scene] will be loaded with any loadings.

----
## How to use and implement startup scripts

* IEnumeratorStartupScript for IEnumerator startup.  
See SSC Sample Scene -> sample ie startup -> SampleIEnumeratorStartupScript

* WwwStartupScript for WWW startup loading.  
See SSC Sample Scene -> sample www startup -> SampleAssetBundleStartupScript

* AssetBundleStartupScript for AssetBundle startup loading.  
See [SSC Sample Scene -> sample ab startup -> SampleAssetBundleStartupScript]  
See [SSC Sample Scene -> sample ab scene startup -> SampleAssetBundleStartupScript]  
(ver2.1) See [SSC Sample Scene -> sample detail sync ab startup -> SampleAssetBundleStartupAsyncScript]  

----
## Customize Startup Manager

Override startup managers.  

See [SSC Demo Init Scene -> Smart Scene Changer Sample -> SampleAssetBundleStartupManager] for AssetBundle startup.  
See [SSC Demo Init Scene -> Smart Scene Changer Sample -> SampleWwwStartupManager] for WWW startup.  
See [SSC Demo Init Scene -> Smart Scene Changer Sample -> SampleIEnumeratorStartupManager] for IEnumerator startup.  

----
## Flow of changing scene

* Start showing nowloading UI
* Start [UnloadUnusedAssets]
* Start [LoadSceneAsync]
* Start loading all of [IEnumeratorStartupScript(before)]
* Start loading all of [AssetBundleStartupScript]
* (If AssetBundle additive scene was detected, restart from IEnumeratorStartupScript(before)])
* Start loading all of [WwwStartupScript]
* Start loading all of [IEnumeratorStartupScript(after)]
* (ver2.1) If new startup objected are detected, restart new all startups.
* Start hiding nowloading UI

----
## Loading AssetBundle in runtime

See [SSC Sample Scene -> Runtime AB Loader Test -> RuntimeLoadABScript]  

----
## CommonUiManager(DontDestroyOnLoad) and SceneUiManager(Not DontDestroyOnLoad)

UI object is controlled by [UiControllScript].
Override [UiControllScript] and add it to CommonUiManager or SceneUiManager with UI identifier you wanted.  

### How to show UI

SSC.CommonUiManager.Instance.showUi("idebtifier", ----);  
SSC.SceneUiManager.Instance.showUi("idebtifier", ----);  

See [SSC Demo Init Scene -> Smart Scene Changer Sample -> CommonUiManager]  
See [SSC Demo Init Scene -> Scene Canvas -> SceneUiManager]  
See [SSC Sample Scene -> Scene Canvas -> SceneUiManager]  
See [SSC Sample Scene -> Scene Canvas -> SampleUITestScript]  

### Selectable

Selectable is used for UI Navigation.  
If valid object is set, the object is selected when showing.  

----
## Customize Dialog

Override [DialogUiControllScript] and add it to DialogManager.  

See [SSC Demo Init Scene -> Smart Scene Changer Sample -> SampleDialogManager]  
See [SSC Demo Init Scene -> Smart Scene Changer Sample -> Canvas -> Dialogs -> Yes No Dialog -> SimpleDialogUiControllerScript]  
See [SSC Demo Init Scene -> Smart Scene Changer Sample -> Canvas -> Dialogs -> Ok Dialog -> SimpleDialogUiControllerScript]  

### How to show dialog

DialogManager.Instance.showOkDialog(----);  
DialogManager.Instance.showYesNoDialog(----);  

----
## Pause state

### Receive

SSC.SimpleReduxManager.Instance.addPauseStateReceiver(<function>);  
See [SSC Sample Scene -> sample ie startup -> SampleIEnumeratorStartupScript]  

### Send

var pState = SimpleReduxManager.Instance.PauseStateWatcher.state();  
pState.setState(SimpleReduxManager.Instance.PauseStateWatcher, bool);  

----
## Tools

* Tools -> SSC -> Editor Scene Loader Window  
Quick scene access tool in editor.

* Tools -> SSC -> Sample Build AssetBundles Window  
A tool to build AssetBundles with encryption.

----
## Tools (ver 2.1)

* Tools -> SSC -> Set AssetBundle Name Window  
A tool to label AssetBundles.

* Tools -> SSC -> Show All AssetBundle Names Window  
A tool to show all AssetBundle names.

* Tools -> SSC -> Create Starter Window  
A tool to create starter managers.

----
## Warnings

* [SmartSceneChangerBase] prefab has [EventSystem], so be careful not to be duplicated.

----
## License

MIT