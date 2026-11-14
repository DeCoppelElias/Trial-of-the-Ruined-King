# Version control

Status: planned, see the [roadmap entry](../roadmap/version-control.md).

## What it does

Decides which version control system is the source of truth, and removes the
other one. The repo has both a `.git` folder with a GitHub remote and a
`.plastic` workspace, plus the Unity Version Control package
(`com.unity.collab-proxy`).

## Why it is interesting

The package is what opens an invisible login dialog in batch mode, so every
command line run must pass `-nographics` or it hangs. Two systems also mean
two histories that can drift. This is the quickest of the architecture topics
and unblocks tooling, so grill it first.

## What it would touch

The `.plastic` folder, the package manifest, `.gitignore`, and the build and
test commands in `AGENTS.md`.

## Open questions

- Git with GitHub, or Unity Version Control? Anything kept in Plastic only?
- Once the package is gone, does `-nographics` stay as a habit or go?
