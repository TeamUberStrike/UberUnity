# EasyAntiCheat (UberStrike) — setup steps

Short version. Full detail is in Epic's docs: <https://dev.epicgames.com/docs/game-services/anti-cheat>

## 1. Epic dev portal (one-time)
- Go to: dev.epicgames.com/portal → org **UberStrikeTeam** → product **UberStike4.3Server**
- Enable it: left sidebar → **Epic Online Services → Anti-Cheat** → turn Anti-Cheat on
- Permission: org admin → give each dev the **Anti-Cheat** role (otherwise the **Integrity keys** button is greyed: "your account does not have access")
- Get keys: **Anti-Cheat → Integrity keys → Download**. You get:
  - `base_private.key` ← **SECRET. Never commit / share.**
  - `base_public.cer` ← the certificate
- Get credentials: **Product Settings** → copy ProductId, SandboxId, DeploymentId, ClientId, ClientSecret

## 2. Unity client (one-time)
- Install the EOS plugin: `com.playeveryware.eos`
- Menu **EOS Plugin → Configuration**:
  - paste ProductId / SandboxId / DeploymentId / ClientId / ClientSecret
  - tick **Use EAC**
  - **Path to EAC private key** = `base_private.key`
  - **Path to EAC Certificate** = `base_public.cer`

## 3. Build
- Build **Windows standalone** (IL2CPP). The integrity tool runs automatically on build and produces **`EACBootstrapper.exe`** + an `EasyAntiCheat/` folder.

## 4. Activate
- Portal → Anti-Cheat: after one client build is launched/registered, **activate the client module**. Then **Client Protection** unlocks → turn it on.

## 5. Test
- Run **`EACBootstrapper.exe`** (not the game .exe).
- If it says "EAC not installed": in that folder, run as admin → `EasyAntiCheat_EOS_Setup.exe install prod-fn`
- Tamper a protected file → relaunch → integrity violation is logged. Done.

> Note: this is **client protection only** (anti-tamper of the real client). It does NOT stop a custom client speaking the protocol — that needs server-side validation (separate work).
