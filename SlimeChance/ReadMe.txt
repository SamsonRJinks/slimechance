===SLIME CHANCE - DEV INSTRUCTIONS===

***Formatting Scene Before Play***

	1. In each scene, there must be a parent object that contains children with the Text Blocks component.
	
	2. The component contains the string MyBlockName, which indicates what the individual TextBlock should be referred to as.
	
	3. It also contains MyBlocks, an adjustable list with individual blocks of dialogue inside of it.
	
	4. Individual Blocks contain the following:
		+Block ID: The tag that indicates the block of text in the chosen text file to be used
		+My Speaker: The image to be used as a speaker in the current block of dialogue
		+My Choices: Indicates the number of choices available in this block (defaults to none)
		+Character Sprites: Any sprites that should be changed in this block (leave blank to turn off a character sprite)
	
	5. On the Canvas object, there are two components: Player Behavior and Stream Reader
		+Display Text and Name Text should be fine as is. 
		+Drag the parent object of the Text Blocks into the Text Blocks Parent field
		+List the Starting Blocks Name string as the first set of Text Blocks you want to access
		+Any text file in the resources folder can be referenced by Stream Reader 
			*(As long as it's in the right format, see below for an example)
		+In Stream Reader, put the name of the text file to be used in Text File Name 
			*(Don't put .txt)
	
	6. Run game. If everything is in order, the game should display your given text file. Click screen to advance.



***Creating Dialogue***

In the .txt file, four lines must be present in each individual block, with a blank break between each.
	
	1. Line 1: #Block_ID#
		+This is the name that will be passed to the Stream Reader dictionary
		+Used in individual blocks to reference these blocks of text in the dictionary

	2. Line 2: Speaker
		+This is the name that will be displayed on the nameplate, to easily keep track of who's speaking

	3. Line 3: Dialogue
		+This is the dialogue itself. It can be as long as desired, so long as there are no line breaks.

	4. Line 4: #End#
		+Ends the block. Must always be followed by a blank line



***Creating Choices***

Choices are similar to the other dialogue blocks, but must follow the below changes.

	1. Line 1: #Block_ID_Choice_1-4#
		+These choices must derive their names from another block, with '_Choice_1-4' listed after the ID
		+The 1-4 references what choice number it is.
			*Ex. With 2 Choices, include #Block_ID_Choice_1# and #Block_ID_Choice_2#

	2. Line 2: Choice Name
		+What will be written on the choice itself. 
		+Also used to reference what new Block set the choice will go to after being selected.
			*My Block Name in Text Blocks component must match this to be successfully transfered to
	
	3. Line 3: Description
		+An extended description of the choice being hovered over.
		+If player stops hovering over a choice, the dialogue box will default back to the original block dialogue
	
	4. Line 4: #End#
		+No changes



***Example .TXT Format***

[EXAMPLE STARTS ON THE NEXT LINE]
#Test_One#
Michiru
I'm saying something here.
#End#

#Test_Two#
Takashi
I'm saying something else.
#End#

#Test_Two_Choice_1#
Choice One
This is the first choice description.
#End#

#Test_Two_Choice_2#
Choice Two
This is the second choice description.
#End#

#Test_Three#
Jelly
Saying a third thing.
#End#
[END EXAMPLE]

		