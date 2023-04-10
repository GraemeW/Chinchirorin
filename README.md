# Chinchirorin

Chinchirorin is a dice game also knowns as [Cee-Lo](https://en.wikipedia.org/wiki/Cee-lo), regularly featured in the [Suikoden](https://en.wikipedia.org/wiki/Suikoden) franchise.

![](/DocResources/Chinchirorin.png)

This Unity project is a 'real physics'-based implementation of said dice game.  Notably, the project is built for WebGL and posts game/match results to javascript, allowing for integration into other javascript platforms.  The default implementation handshakes with a DiscordBot hosted on the same server.

## Running Chinchirorin Locally

In order to run a local web server to test the OpenGL-compiled version of the project, one can use [python](https://www.python.org/), specifically calling the [http.server](https://docs.python.org/3/library/http.server.html) function. 

For example:
* Navigate to the build directory:

    ```cd <path>\Chinchirorin\BuildsWebGL```

* Start a web server:

    ```python -m http.server 80```

* Open a web browser, and navigate to: localhost

![](/DocResources/ChinchirorinInFirefox.png)

## Javascript Handshaking

The [DiscordHandshaker](/Assets/Scripts/Utility/DiscordHandshaker.cs) class listens and posts messages via [DiscordJSLibrary.jslib](/Assets/Plugins/DiscordJSLibrary.jslib) on '/chinchirorin/roll' per:
```
PostRollData: function (payload) {
    fetch('/chinchirorin/roll', {method: 'POST', body: UTF8ToString(payload)});
  }
```

In this manner, the [DiscordHandshaker](/Assets/Scripts/Utility/DiscordHandshaker.cs) handles:
* Game setup 
* Posting roll results
* Posting game results

, by passing encrypted json-formatted [MatchData](/Assets/Scripts/Score/MatchData.cs) objects back and forth.  Data is encrypted using a standard symmetric encryptor implementation via the [SymmetricEncryptor](/Assets/Scripts/Utility/SymmetricEncryptor.cs) class.

MatchData objects track the following properties:
* string:  matchResolutionType
    * derived from: [MatchResolutionType](/Assets/Scripts//Score/MatchResolutionType.cs)
* int: betWinnings
* string:  playerName
* int: playerScore
* string:  opponentName
* int: opponentScore
* public: int throwCountTracker
* public: long timeStamp

### Game Setup

In order to initialize the game on WebGL, the following edits can be made to the auto-generated index.html file in the script.onload listener.  Notably, the Setup() function is called on [DiscordHandshaker](/Assets/Scripts/Utility/DiscordHandshaker.cs) with an encrypted [MatchData](/Assets/Scripts/Score/MatchData.cs) token to initialize gameplay.

```js
      script.onload = () => {
        createUnityInstance(canvas, config, (progress) => {
          progressBarFull.style.width = 100 * progress + "%";
        }).then((unityInstance) => {
          unityInstance.SendMessage('DiscordHandshaker', 'Setup', 'Jym/DRAwKgKTQg54aL/j1mmKZq/gDKr55Z5mvU8N3/wkOj6wV+k5EQ4L+V2xTo+BMEY5Cjyb5mv+ktAKamKoIdJK9ifxDjexFDhEEIzlmWn/WvsOSvbWBSrp5jodFWMOP4dFzQ4t+jSornuF7EbmQTWJiUIhF/b3zWDTJGlSJ7+rQjWAgjimmQoIXIymtPBg/IS1fPOLxgVBa/oR7wmRJgcxb24g0p61Vxq/eHY0kPo=');

          loadingBar.style.display = "none";
          fullscreenButton.onclick = () => {
            unityInstance.SetFullscreen(1);
          };
        }).catch((message) => {
          alert(message);
        });
      };
```
, where `Jym/DRAwKgKTQg54aL/j1mmKZq/gDKr55Z5mvU8N3/wkOj6wV+k5EQ4L+V2xTo+BMEY5Cjyb5mv+ktAKamKoIdJK9ifxDjexFDhEEIzlmWn/WvsOSvbWBSrp5jodFWMOP4dFzQ4t+jSornuF7EbmQTWJiUIhF/b3zWDTJGlSJ7+rQjWAgjimmQoIXIymtPBg/IS1fPOLxgVBa/oR7wmRJgcxb24g0p61Vxq/eHY0kPo=` is the example token.

For final integration with a paired javascript platform (e.g. such as a Discord Bot), this token should be replaced with the per-user data/match information.  Specifically, make a new MatchData with playerName and the bet.

### Posting Roll & Game Results

Following the above description of handshaking, game data are posted after every roll, with game resolution defined by [MatchResolutionType](/Assets/Scripts//Score/MatchResolutionType.cs).  As long as the game is still in progress, the tokens posted will have a resolution type of:  InProgress.

For integration with the paired javascript platform, the following tasks are expected to be completed by the server-side javascript:

* Listen for POST data from the WebGL interface on '/chinchirorin/roll'
* Decrypt the data - match AES-encryption via [SymmetricEncryptor](/Assets/Scripts/Utility/SymmetricEncryptor.cs)
    * Adjust the static password code accordingly (and do NOT save to GIT as done here)
* Unpack the json [MatchData](/Assets/Scripts/Score/MatchData.cs) string
* Handle errors / abnormal data
    * i.e. ignore malformed tokens, ignore repeat rolls, ignore multiple wins, etc.
* Track the session in the server-side match database (update user wallet, match history, etc.)
    * pending the [MatchData](/Assets/Scripts/Score/MatchData.cs) containing a relevant [MatchResolutionType](/Assets/Scripts//Score/MatchResolutionType.cs) - PlayerWin, PlayerLoss, Draw

## Debug Notes:

### Play Again

The “Play Again” button after completing a full match is included for debug.  After full integration with a paired javascript platform, this should be removed (i.e. game initialization should only be possible via javascript calls to the DiscordHandshaker, as above).

### Unity Execution

In order to test game functionality in Unity, the 'Debug Generate Test Initiator' bool in the [DiscordHandshaker](/Assets/Scripts/Utility/DiscordHandshaker.cs) Game Object should be set to:  ENABLE
