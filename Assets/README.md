# Void Lands - Project Organization

This document describes the organized folder structure for the Void Lands Unity project.

## Folder Structure

### 01_Scripts/
Contains all C# scripts organized by functionality:
- **Core/** - Core game systems and managers
- **Player/** - Player-related scripts (movement, combat, etc.)
- **Enemies/** - Enemy AI and behavior scripts
- **UI/** - User interface scripts
- **Items/** - Item and inventory system scripts
- **Skills/** - Skill system scripts
- **Managers/** - Game managers and controllers
- **Utilities/** - Helper scripts and utilities
- **Editor/** - Unity Editor scripts

### 02_Prefabs/
Contains all prefabs organized by type:
- **Player/** - Player character prefabs
- **Enemies/** - Enemy prefabs
- **UI/** - UI prefabs (canvases, panels, etc.)
- **Items/** - Item prefabs (chests, loot, etc.)
- **Environment/** - Environmental prefabs
- **Effects/** - Visual effects prefabs
- **Cameras/** - Camera prefabs

### 03_Materials/
Contains all materials and shaders:
- Materials (.mat files)
- Shaders (.shader files)

### 04_Textures/
Contains all texture assets:
- **Characters/** - Character sprites and textures
- **UI/** - UI textures and icons
- **Tilesets/** - Tile textures for levels
- **Gear/** - Equipment textures
- **Areas/** - Area/level textures
- **Enemies/** - Enemy sprites
- **Bosses/** - Boss sprites
- **Skills/** - Skill effect textures
- **Items/** - Item sprites
- **Consumables/** - Consumable item sprites
- **SpecialItems/** - Special item sprites
- **Gold&Stuff/** - Currency and miscellaneous sprites

### 05_Audio/
Contains all audio assets:
- **Music/** - Background music
- **SFX/** - Sound effects
- **UI_Sounds/** - UI sound effects
- **Ambient/** - Ambient sounds
- **Voice/** - Voice lines

### 06_Scenes/
Contains all Unity scenes

### 07_Animations/
Contains all animation files and controllers

### 08_UI/
Contains UI-specific assets:
- **Icons/** - UI icons (skill icons, etc.)
- **Fonts/** - Font files
- **Buttons/** - Button assets
- **Panels/** - Panel assets
- **GUI-Elements/** - General UI elements
- **SkillTooltipTools/** - Skill tooltip assets

### 09_VFX/
Contains visual effects assets

### 10_ThirdParty/
Contains third-party assets and plugins:
- **FirstGearGames/** - First Gear Games assets
- **MoreAssets/** - Additional third-party assets
- **StoreAssets/** - Unity Asset Store assets

### 11_Documentation/
Contains project documentation

### 12_Archived/
Contains deprecated or unused assets:
- **Stuff/** - Miscellaneous old assets
- **NotUsed/** - Unused assets

## Naming Conventions

- Use PascalCase for folder names
- Use descriptive names that clearly indicate content
- Prefix main folders with numbers for consistent ordering
- Group related assets together

## Best Practices

1. **Keep Assets Organized**: Always place new assets in the appropriate folder
2. **Use Descriptive Names**: Name files and folders clearly
3. **Avoid Deep Nesting**: Don't create too many subfolder levels
4. **Consistent Structure**: Follow the established pattern for new content
5. **Regular Cleanup**: Periodically move unused assets to the Archived folder

## Migration Notes

This structure was created to replace the previous disorganized folder layout that included:
- Loose files in Assets root
- Poorly named folders (Stuff, MoreAssets, NotUsed)
- Mixed content types in single folders
- Inconsistent naming conventions

All assets have been moved to their appropriate locations while maintaining their original functionality. 