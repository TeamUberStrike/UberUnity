# EAC Branch Notes

## Initial 56-error compile failure

On first clone this branch failed to compile with 56 cascading `CS0246` errors for `EOSManager`, `Epic.OnlineServices.*`, `ProductUserId`, `P2PInterface`, `PacketReliability`, `NATType`, `SocketId`, etc. Root cause: `Packages/manifest.json` references the EOS plugin via `file:../com.playeveryware.eos-5.1.2.tgz`, but the tarball itself was never committed (it isn't in `.gitignore`, just absent), so Unity Package Manager couldn't resolve `com.playeveryware.eos` and every script that imports its namespaces failed in cascade. Fix: download `com.playeveryware.eos-5.1.2.tgz` (147 MB, sha256 `91e671f246947853370e2eef6eed7c120a33b8c0714909ad9a71e103d84a17a5`) from <https://github.com/EOS-Contrib/eos_plugin_for_unity/releases/tag/v5.1.2> and drop it at the repo root — errors clear on next domain reload.
