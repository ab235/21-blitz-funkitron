=============================
 SegmentDisplay - ReadMe.txt
=============================



Introduction
============


SegmentDisplay is classic display where each digit is made of segments that turn on and off, forming numbers or letters.

This package contains multiple different type and style displays that can be embedded to your game objects or UI.
Content of the display can be then changed from code during runtime.



Adding SegmentDisplay
=====================


See folder SegmentDisplay/Prefabs/  There is 2 different prefabs:

- "SegmentDisplay_World" is sprite based display to be used in 2D or 3D game world.
- "SegmentDisplay_UI" is image based display to be used as UI element.

Simply drag wanted prefab to your Scene view or Hierarchy window.

Note, if you are using UI-version of prefab: When dragging prefab to hierarchy or scene, prefab will automatically set UI canvas
as its parent. If there is no other UI elements and no UI Canvas in scene yet, one is created automatically. However, it's
recommended to first create UI Canvas using normal Unity Editor tools before adding UI-version of SegmentDisplay to the scene.

Changing actual visual size of display depends on which prefab was used.

To change size of display made of "SegmentDisplay_World", simply change Scale in Transform.

Display made of "SegmentDisplay_UI" instead will always fit itself inside the rect defined by RectTransform. So there are multiple
settings like anchors, pivot points, width/height that defines the size of display. And size may change when resolution or aspect ratio
changes. Just handle the display like any other UI element.




Modifying SegmentDisplay
========================


Select just added SegmentDisplay object from hierarchy. Inspector window will show you controls to personalize the display for
your needs. If you feel uncertain of meaning of some setting, you can tap on "Enable inspector help texts" that is at the end of
SegmentDisplay settings in Inspector window.



Accessing SegmentDisplay
========================


Create new script and make sure it is "using Leguar.SegmentDisplay;". Use any way you prefer to give your own script reference to
SegmentDisplay script in gameobject created earlier.


You can then change text of display simply by writing:

mySegmentDisplay.SetText("42");

This will cause text "42" to appear on display immediately.


If you need more exact control of display you can access single digits of display:

SingleDigit displayDigit = mySegmentDisplay[0];
displayDigit.SetChar('8');

This will set number 8 to leftmost digit of display without changing any other digits.


If you need more complex and automated control, you can use SDController class. For example adding scrolling text:

SDController sdController = mySegmentDisplay.GetSDController();
ScrollTextCommand scrollCommand = new ScrollTextCommand("Hello World", ScrollTextCommand.Methods.StartOutsideMoveLeftUntilOut, 3f);
sdController.AddCommand(scrollCommand);

This will scroll text "Hello World" from right to left, with speed 3 digits per second.

Multiple commands can be added to SDController at once, they will be executed in order they were added.

List of different commands:

ClearCommand - Clear display or part of it
FillCommand - Fill display or part of it
SetTextCommand - Set text to display (all digits not used by text will be cleared)
AddTextCommand - Add text to display (all digits not used by text will stay unchanged)
ScrollTextCommand - Scroll text on display
PauseCommand - Delays SDController for certain amount of time
CallbackCommand - Calls certain function

See also the example scenes and scripts to see how these are used.



Examples and additional documents
---------------------------------

Folder SegmentDisplay/Examples/ contains multiple example scenes with different SegmentDisplays doing different things. Take a look
and also check the example c# codes in same folder. Feel free to modify and use any parts of these examples in your own projects.

Inline c# documents are included to all public classes. They are also available online in html format:

http://www.leguar.com/unity/segmentdisplay/apidoc/1.3/



Feedback
========


If you are happy with this asset, please rate us or leave feedback in Unity Asset Store:

https://assetstore.unity.com/packages/slug/119005


If you have any problems, or maybe suggestions for future versions, feel free to contact:

http://www.leguar.com/contact/?about=segmentdisplay
