# WAX SDK for Unity

This package includes base implementation for EOSIO blockchain and several other providers and SDKs all in one:

- Anchor Link
- WCW (WAX Cloud Wallet)
- Atomic Assets
- EOSIO blockchain

## Requirements

As this repo is pre packaging into managed Dlls you are required to install a number of libraries in order for the
SDK to function, please install the following packages in the following order using the package manager within Unity as the installer window is added during the packaging stage.

- [BouncyCastle](https://www.bouncycastle.org/csharp/index.html) (included with package)
- [ZXing.NET](https://github.com/micjahn/ZXing.Net) (included with package)
- [UniTask](https://github.com/Cysharp/UniTask)
- [Newtonsoft.Json](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@latest) `com.unity.nuget.newtonsoft-json`
- [NativeWebSocket](https://github.com/endel/NativeWebSocket)

## Setup

Here are some additional setup required for specific services.

### WAX Cloud Wallet

#### Example

There is a sample included for Logging into the WAX Cloud Wallet and then Signing transactions using it.

The example is available at **Assets/Plugins/WaxUnity/Example**

#### Browser Setup

There are specific integrations included for the WAX Cloud Wallet, because this service requires an embedded browser inside the app or game for login and signing of transactions, you can find these integrations at **Plugins/WaxUnity/Integrations**.

The included integrations are:

- **Vuplex**: Integration package for the [Vuplex 3D WebView](https://assetstore.unity.com/publishers/40309)
- **ZFBrowser**: Integration package for the [Zen Fulcrum Embedded Browser](https://assetstore.unity.com/packages/tools/gui/embedded-browser-55459)
- **UnityWebBrowser**: Integration package for the open source web browser solution, [Voltstro's Unity Web Browser](https://github.com/Voltstro-Studios/UnityWebBrowser)
- **WebGL**: Integration package for WebGL builds

If you're building for WebGL, you can use the WebGL integration to use the browser itself for processing the requests.

**Note**: The WebGL integration includes a custom template which is required for the integration package to work, so after importing the integration package, make sure to select WaxDefault as template for your WebGL, or if you want to use your own custom template or any other third-party template, make sure to set `unityGame` variable to `unityInstance` in the `index.html` of the template, for example, the Default template does not assign the unityInstance variable to a global variable, so all you need to is to assign the `unityGame` variable to it which should be defined at the global scope.

Here is a sample:

```csharp
    var script = document.createElement("script");
    var unityGame; // define the variable at global scope
    script.src = loaderUrl;
    script.onload = () => {
      createUnityInstance(canvas, config, (progress) => {
        progressBarFull.style.width = 100 * progress + "%";
      }).then((unityInstance) => {
        unityGame = unityInstance; // assign unity instance to it
        loadingBar.style.display = "none";
        fullscreenButton.onclick = () => {
          unityInstance.SetFullscreen(1);
        };
      }).catch((message) => {
        alert(message);
      });
    };
    document.body.appendChild(script);
```

So if you want to use a custom template, make sure to assign the variable called `unityGame` to `unityInstance` in order for the integration to work.

### Anchor Link

#### Example

There is a sample included for Logging into the Anchor Link and then Signing transactions using it.

The example is available at **Assets/Plugins/AnchorLinkUnity/Example**

#### Customization

You can customize the dialog and other UIs inside the **Assets/Plugins/AnchorLinkUnity/Resources/AnchorLink** folder using the `AnchorLinkCanvas` prefab.

The animations are available at **Assets/Plugins/AnchorLinkUnity/Animations** folder.

### Atomic Assets

#### Example

There are 2 sample included, one is a minimal Get API example and the other one is an Inventory demo.

The examples are available at **Assets/Plugins/AtomicAssetsUnityClient/Example**

### EOS

#### Example

There are 2 samples included one is for encoding an EOS Signing Request and the other one is for decoding an EOS Signing Request.

The examples are available at **Assets/Plugins/EosUnity/Example**

## Credits

Check the [Third Party Notices](https://github.com/WAX-SDK-wax-labs-proposal/wax-unity-sdk/blob/main/Third%20Party%20Notices.md) for more information.
