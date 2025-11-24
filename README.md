## Project Setup

- This repository uses Git LFS. Install it [here](https://git-lfs.com/) and afterwards run git lfs pull to pull all assets
- Install Godot v4.5.1 with C# Support. [Link for Linux](https://downloads.godotengine.org/?version=4.5.1&flavor=stable&slug=mono_linux_x86_64.zip&platform=linux.64)
- Open the project in godot and done

## Editor Setup

- VSCode:
  - Install the C# extension. That should suffice
- Neovim:
  - If version >= 0.11.0:
      ```lua
      vim.lsp.enable("csharp_ls")
      ```
  - If version below, setup LSP server `csharp_ls` like other LSP servers in your config
  - Regardless of version, install the lsp server as documented [here](https://github.com/razzmatazz/csharp-language-server?tab=readme-ov-file#quick-start)

## Scope

We don't want to overscope at first, so:

- player movement with camera mouse
- a small island
- few dirtpatches where you can put crops to
- they grow over time
- when they are done growing, a npc arrives and wants to buy the plant
- you have to sell it to him and you get coins
- be able to buy new plots for plants and have different types of plants

## Assets

- 3D Models: Please use `glb` format if possible
