# Third-party attribution

## Folia

Original project: https://github.com/chthollyphile/folia-major

Original author: chthollyphile. Original contributors are listed in CONTRIBUTORS.md. This unofficial derivative is based on commit d695db52a313abe2077ce4c2e5894cc4a82190f1 and retains the full GNU AGPL v3 text in LICENSE. Changes dated 2026-11-01 add Windows taskbar lyrics, animated desktop wallpaper rendering, and tray/taskbar controls.

## TaskbarMusic V1 and AgentSpeed

The native taskbar card, renderer, and Win32 integration are adapted from the supplied TaskbarMusic V1 implementation. The accompanying source documentation describes it as recovered from its release binary and references AgentSpeed for the taskbar component. The available AgentSpeed notice is retained in licenses/AgentSpeed-MIT.txt. It applies to the relevant AgentSpeed-derived material and does not establish an MIT license for all TaskbarMusic code.

AgentSpeed upstream: https://github.com/zoefix/agent-speed

Copyright (c) 2026 zoefix. Full MIT license: [licenses/AgentSpeed-MIT.txt](licenses/AgentSpeed-MIT.txt).

## Other dependencies

The Windows packages include an MNN runtime built from Alibaba MNN commit `d407447ed56c4121a11ccbd266dc184ca1ead0c2`, with Folia's UTF-8 file-path adjustment described in `build/buildMnnRuntime.mjs`. Its Apache 2.0 license is retained in [licenses/MNN-Apache-2.0.txt](licenses/MNN-Apache-2.0.txt).

The bundled analysis weights are identified and hash-verified by `shared/modelManifest.json`. Beat This! is credited to the Institute of Computational Perception, JKU Linz; its MIT notice is retained in [licenses/Beat-This-MIT.txt](licenses/Beat-This-MIT.txt). The htdemucs weights derive from Demucs by Meta Platforms, Inc. and affiliates; its MIT notice is retained in [licenses/Demucs-MIT.txt](licenses/Demucs-MIT.txt). The FFmpeg runtime ships its own redistribution metadata under `resources/ffmpeg-audio/share/folia-ffmpeg`.

Existing third-party notices, source comments, dependency manifests, and bundled license files are retained. Folia uses the [AMLL TTML lyrics database](https://github.com/amll-dev/amll-ttml-db), [LDDC](https://github.com/chenmozhijin/LDDC), [NeteaseCloudMusicApiEnhanced](https://github.com/NeteaseCloudMusicApiEnhanced/api-enhanced), [pretext](https://github.com/chenglou/pretext), [KuGouMusicApi](https://github.com/MakcRe/KuGouMusicApi), [paper-design/shaders](https://github.com/paper-design/shaders), and [qq-music-api](https://github.com/yakult-green-tea/qq-music-api). Their respective licenses and content rights continue to apply.
