# Lonewolf Workshop Uploader

Tool to upload any folder as steam workshop item for Homeworld Remastered Collection.

#### Usage

Run in command line.

```powershell
LonewolfWorkshopUploader.exe buildconfig_example.json
```

---

To give some context here, for long we have the need of a tool to upload custom files to workshop, because:

* we need to upload the English and Chinese versions of our mods as 2 separate items in workshop. These 2 items have a same large main .big file with different small localization .big files. Using the vanilla WorkshopTool means we need to build the same large .big 2 times. And sometimes we also want to release a standalone mod installer for those who play our mod on GOG, in such cases we need to build the same file 3 times, which is very time-consuming.
* we also need to upload cutscene videos of our mod, which the vanilla WorkshopTool does not support.
For a long time, we used Valve’s steamcmd tool to achieve this (You can find a tutorial about this here Steam Community :: Guide :: Uploading & Using Steam Workshop for QuakeLive). But steamcmd has some bugs like it won’t update our mods’ workshop thumbnail, and it logs me out of my steam on my PC whenever I use it.

So I have decided to write my own tool to do this - and it turns out to be much easier than I thought and works like a charm.

By the way, if you are interested in this tool, you may also want to checkout me previously released [LoneWolf Archiver - A fast and open-sourced alternative to Archive.exe](https://github.com/lone-wolf-akela/LoneWolf_Archiver).
