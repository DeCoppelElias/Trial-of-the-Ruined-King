# Unity MCP

Status: idea.

## What it does

Connects an AI agent to the open Unity Editor through an MCP server, so the
agent can enter and exit Play Mode, read the console, find and change
GameObjects and components in a scene, open and save scenes and prefabs, run
tests, and capture the Game or Scene view as an image to compare with an
earlier one.

## Why it is interesting

Today an agent only sees code and batchmode test results. With the editor in
reach it can check a change in the running game, adjust a scene without hand
editing YAML, and compare screenshots before and after a visual change.

## What it would touch

The `com.unity.ai.assistant` package, already installed at 1.6.0-pre.3, which
ships a Unity MCP server. An `.mcp.json` at the repo root for Claude Code. The
build and run section of `AGENTS.md`, which assumes the editor is closed for
terminal runs.

## Open questions

- The official server in `com.unity.ai.assistant`, or a community one such as
  `CoplayDev/unity-mcp`, `CoderGamester/mcp-unity` or `IvanMurzak/Unity-MCP`?
  The official one is installed, the community ones may offer more tools.
- How it lives beside batchmode runs, which need the editor closed.
- Screenshot comparison outside the editor. Batchmode with `-nographics`
  renders nothing, so a capture there needs graphics (and a way past the
  Version Control login dialog) or a built player.
- Comparison by eye from saved PNGs, or pixel checks against reference images
  with `com.unity.testframework.graphics`?
