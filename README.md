# Setup

LightingData related files may change depending on the operating system. It is good practice to tell git to ignore them.

```
git update-index --skip-worktree Assets/Maps/Danger_Zone/Danger_Zone/LightingData.asset
git update-index --skip-worktree Assets/Maps/Danger_Zone/Danger_Zone/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Maps/Danger_Zone/Danger_Zone/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Maps/Monkey_Island/Monkey_Island/LightingData.asset
git update-index --skip-worktree Assets/Maps/Monkey_Island/Monkey_Island/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Maps/Monkey_Island/Monkey_Island/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Maps/Research_Hub/Research_Hub/LightingData.asset
git update-index --skip-worktree Assets/Maps/Research_Hub/Research_Hub/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Maps/Research_Hub/Research_Hub/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Maps/Space_City/Space_City/LightingData.asset
git update-index --skip-worktree Assets/Maps/Space_City/Space_City/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Maps/Space_City/Space_City/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Maps/The_Hangar/The_Hangar/LightingData.asset
git update-index --skip-worktree Assets/Maps/The_Hangar/The_Hangar/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Maps/The_Hangar/The_Hangar/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Menus/Intro/LightingData.asset
git update-index --skip-worktree Assets/Menus/Intro/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Menus/Intro/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Menus/MainMenu/LightingData.asset
git update-index --skip-worktree Assets/Menus/MainMenu/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Menus/MainMenu/ReflectionProbe-0.exr.meta
git update-index --skip-worktree Assets/Menus/Intro/LightingData.asset
git update-index --skip-worktree Assets/Menus/Intro/ReflectionProbe-0.exr
git update-index --skip-worktree Assets/Menus/Intro/ReflectionProbe-0.exr.meta
```

# Functionality

The IP address is hard coded to: 127.0.0.1

You can enter a match and send/receive chat messages using two clients and a server.
Steps:
- Start the server
- enter a match by New Game -> select Map -> select Exploration -> start
- send and receive chat messages

Furthermore data about weapon changes, firing and more is sent to the server.
