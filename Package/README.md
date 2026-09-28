# Backpack Tombstone

Adventure Backpacks empties a backpack onto the ground whenever the backpack item is removed, including when death builds your tombstone. The grave still holds the backpack with those items inside, so the ground copies are duplicates.

This skips that dump while you are dying, and again when you take the backpack back out of the tombstone. The grave keeps one copy of whatever was inside. Dropping or destroying a backpack any other time still empties it the way Adventure Backpacks always has.

Install on **clients**. A dedicated server does not run the backpack dump.
Drop `Cjayride.BackpackTombstone.dll` into `BepInEx/plugins`.
