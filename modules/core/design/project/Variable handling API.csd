<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Comment">
      <Text>Variables are data elements. They don't actually exist as a single class / object. They are more a collection of objects just like components

Variables should be stored with the data set.

Mark variables as stored / calculated to indicate how the value is determined.

Provide variable value collections that can store values for the variables. These collections can be global or local. 
Either store the cached value of the equation or just their value. Need to have some update mechanism that allows indication of cache invalidity.</Text>
      <Location left="5" top="5" />
      <Size width="357" height="240" />
    </Entity>
    <Entity type="Class">
      <Name>Alias</Name>
      <Access>Public</Access>
      <Location left="558" top="138" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>VariableId</Name>
      <Access>Public</Access>
      <Location left="5" top="258" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Calculator</Name>
      <Access>Public</Access>
      <Location left="822" top="689" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Complemental</Name>
      <Access>Public</Access>
      <Location left="711" top="483" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Defines zero or more complemental variables + an equation or calculator that can transfer between the two variables</Text>
      <Location left="1046" top="483" />
      <Size width="183" height="85" />
    </Entity>
    <Entity type="Comment">
      <Text>Defines zero or more calculators that are used to provide values for the variable based on the presence of other variables</Text>
      <Location left="1046" top="689" />
      <Size width="183" height="85" />
    </Entity>
    <Entity type="Comment">
      <Text>Indicates which other variables are equivalent to this variable. 

Equivalent variables can share information?</Text>
      <Location left="800" top="138" />
      <Size width="235" height="85" />
    </Entity>
    <Entity type="Class">
      <Name>Expression</Name>
      <Access>Public</Access>
      <Location left="586" top="689" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Defines zero or more expressions that can be used to determine the value of the variable.
Can work together with a calculator.</Text>
      <Location left="311" top="689" />
      <Size width="208" height="85" />
    </Entity>
    <Entity type="Class">
      <Name>Symbol</Name>
      <Access>Public</Access>
      <Location left="5" top="355" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Range</Name>
      <Access>Public</Access>
      <Location left="558" top="5" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Defines the range of the variable. Can be:
- x &lt; range &lt; y
- always positive
- Always smaller than x
- Set of values (e.g. all odd numbers between 4 and 21)
- Expression</Text>
      <Location left="800" top="5" />
      <Size width="235" height="122" />
    </Entity>
    <Entity type="Class">
      <Name>Location</Name>
      <Access>Public</Access>
      <Location left="558" top="238" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Location indicates if a variable is stored or calculated.

Calculated variables can provide a use-caching equation which will be used to determine if caching the value is useful.</Text>
      <Location left="800" top="238" />
      <Size width="235" height="105" />
    </Entity>
    <Entity type="Comment">
      <Text>How about:
- Transformation between different variables?
- Should all variables store all information, or do we have 1 master variable (which stores all the info) and then slave variables (that are just aliases)</Text>
      <Location left="225" top="458" />
      <Size width="253" height="107" />
    </Entity>
    <Entity type="Comment">
      <Text>Stores unit</Text>
      <Location left="225" top="372" />
      <Size width="99" height="50" />
    </Entity>
    <Entity type="Comment">
      <Text>Provide directed variables as well --&gt; variables that are linked to a tensor system (e.g. a coordinate system). Provide information about which part of the tensor the variables is linked to</Text>
      <Location left="5" top="458" />
      <Size width="207" height="107" />
    </Entity>
    <Entity type="Comment">
      <Text>Define variables to be tensors. Define size (scalar, vector, tensor + size). Allow definition of sub-variables which define the individual tensor elements</Text>
      <Location left="5" top="575" />
      <Size width="207" height="93" />
    </Entity>
    <Entity type="Comment">
      <Text>Provide caching decision mechanisms. Need a way to determine if we should cache.

Also indicate dependencies?</Text>
      <Location left="225" top="575" />
      <Size width="253" height="93" />
    </Entity>
    <Entity type="Comment">
      <Text>How do we deal with error information. We should be able to set a tolerance on each variable.

Also we'll need some way of storing the error with the variable (e.g. +- 0.01)</Text>
      <Location left="5" top="689" />
      <Size width="207" height="122" />
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Comment" first="5" second="4">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1021</X>
        <Y>518</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>898</X>
        <Y>518</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="4" second="3">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>898</X>
        <Y>548</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>928</X>
        <Y>664</Y>
      </BendPoint>
      <Direction>Bidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="6" second="3">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1021</X>
        <Y>734</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1009</X>
        <Y>734</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="7" second="1">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>775</X>
        <Y>180</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>745</X>
        <Y>180</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="4" second="8">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>686</X>
        <Y>548</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>652</X>
        <Y>664</Y>
      </BendPoint>
      <Direction>Bidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="9" second="8">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>544</X>
        <Y>733</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>561</X>
        <Y>733</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="8" second="3">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>773</X>
        <Y>733</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>797</X>
        <Y>733</Y>
      </BendPoint>
      <Direction>Bidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="12" second="11">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>775</X>
        <Y>51</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>745</X>
        <Y>51</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="14" second="13">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>775</X>
        <Y>280</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>745</X>
        <Y>280</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="16" second="10">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>200</X>
        <Y>410</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>192</X>
        <Y>376</Y>
      </BendPoint>
    </Relation>
  </Relations>
</ClassProject>