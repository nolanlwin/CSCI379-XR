# CSCI379-XR

## Project Reflection

This Unity XR project represents my exploration into developing a multiplayer first-person shooter experience with extended reality capabilities. The project integrates Photon Fusion for networked gameplay, Unity's XR Interaction Toolkit, and various gameplay systems including weapons, player health, and UI management.

## Setup Instructions

### Required: Photon Fusion App ID

Before running this project, you must configure your Photon Fusion App ID:

1. **Create a Photon Account**: Sign up at https://dashboard.photonengine.com/
2. **Create a Fusion 2 Application**: 
   - Log into the Photon Dashboard
   - Click "Create a New App"
   - Select "Fusion" as the product type
   - Choose **"Fusion 2"** (not Fusion 1)
   - Enter an app name and create the application
3. **Copy Your App ID**: The App ID will be displayed (format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)
4. **Add App ID to Project**:
   - Open Unity and navigate to `Tools > Fusion > Fusion Hub`
   - Paste your App ID into the "Fusion App Id" field
   - Alternatively, you can manually edit `Assets/Photon/Fusion/Resources/PhotonAppSettings.asset` and set the `AppIdFusion` field

**Note**: Without a valid Fusion App ID, the project will fail to connect to Photon's networking services and you'll see an error: `[Fusion] GetRegions failed. Can't provide regions list. ReturnCode: -2: Empty application id`

### Key Features

- **Multiplayer Deathmatch**: Real-time networked gameplay using Photon Fusion
- **XR/VR Support**: OpenXR and Oculus integration for immersive experiences
- **Weapon System**: Multiple weapon types (Pistol, Rifle, Shotgun) with firing mechanics, ammo management, and reload systems
- **Player Systems**: Health management, respawn mechanics, and player statistics tracking
- **UI Framework**: Comprehensive menu system, scoreboard, game over screens, and in-game HUD

### Learning Experience

Working on this project provided valuable insights into networked game development and XR integration. Implementing synchronized gameplay across multiple clients required careful consideration of network architecture and state management. The XR components added another layer of complexity, requiring adaptation of traditional FPS mechanics for immersive environments.

### Challenges & Solutions

One of the main challenges was ensuring smooth network synchronization while maintaining responsive gameplay. Using Photon Fusion's networked objects and state authority helped manage this complexity. Additionally, adapting the UI and interaction systems for XR required rethinking traditional input methods and player feedback mechanisms.

### Demo Video

[Video Link Placeholder - Insert your demo video URL here]

---

*This project was developed as part of CSCI379 - Extended Reality course at Bucknell University.*