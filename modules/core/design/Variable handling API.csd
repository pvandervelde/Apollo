<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Comment">
      <Text>Variables are data elements. They don't actually exist as a single class / object. They are more a collection of objects just like components

Variables should be stored with the data set.

Mark variables as stored / calculated to indicate how the value is determined.

Provide variable value collections that can store values for the variables. These collections can be global or local. 
Either store the cached value of the equation or just their value. Need to have some update mechanism that allows indication of cache invalidity.</Text>
    </Entity>
    <Entity type="Class">
      <Name>Alias</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>VariableId</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Calculator</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Complemental</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Defines zero or more complemental variables + an equation or calculator that can transfer between the two variables</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Defines zero or more calculators that are used to provide values for the variable based on the presence of other variables</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Indicates which other variables are equivalent to this variable. 

Equivalent variables can share information?</Text>
    </Entity>
    <Entity type="Class">
      <Name>Expression</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Defines zero or more expressions that can be used to determine the value of the variable.
Can work together with a calculator.</Text>
    </Entity>
    <Entity type="Class">
      <Name>Symbol</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Range</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Defines the range of the variable. Can be:
- x &lt; range &lt; y
- always positive
- Always smaller than x
- Set of values (e.g. all odd numbers between 4 and 21)
- Expression</Text>
    </Entity>
    <Entity type="Class">
      <Name>Location</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Location indicates if a variable is stored or calculated.

Calculated variables can provide a use-caching equation which will be used to determine if caching the value is useful.</Text>
    </Entity>
    <Entity type="Comment">
      <Text>How about:
- Transformation between different variables?
- Should all variables store all information, or do we have 1 master variable (which stores all the info) and then slave variables (that are just aliases)</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Stores unit</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Provide directed variables as well --&gt; variables that are linked to a tensor system (e.g. a coordinate system). Provide information about which part of the tensor the variables is linked to</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Define variables to be tensors. Define size (scalar, vector, tensor + size). Allow definition of sub-variables which define the individual tensor elements</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Provide caching decision mechanisms. Need a way to determine if we should cache.

Also indicate dependencies?</Text>
    </Entity>
    <Entity type="Comment">
      <Text>How do we deal with error information. We should be able to set a tolerance on each variable.

Also we'll need some way of storing the error with the variable (e.g. +- 0.01)</Text>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Comment" first="5" second="4" />
    <Relation type="Association" first="4" second="3">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="6" second="3" />
    <Relation type="Comment" first="7" second="1" />
    <Relation type="Association" first="4" second="8">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="9" second="8" />
    <Relation type="Association" first="8" second="3">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="12" second="11" />
    <Relation type="Comment" first="14" second="13" />
    <Relation type="Comment" first="16" second="10" />
  </Relations>
  <Positions>
    <Shape>
      <Location left="5" top="5" />
      <Size width="357" height="240" />
    </Shape>
    <Shape>
      <Location left="561" top="138" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="5" top="258" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="822" top="689" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="711" top="483" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="1046" top="483" />
      <Size width="183" height="85" />
    </Shape>
    <Shape>
      <Location left="1046" top="689" />
      <Size width="183" height="85" />
    </Shape>
    <Shape>
      <Location left="800" top="138" />
      <Size width="235" height="85" />
    </Shape>
    <Shape>
      <Location left="586" top="689" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="311" top="689" />
      <Size width="208" height="85" />
    </Shape>
    <Shape>
      <Location left="5" top="355" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="558" top="5" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="800" top="5" />
      <Size width="235" height="122" />
    </Shape>
    <Shape>
      <Location left="561" top="238" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="800" top="238" />
      <Size width="235" height="105" />
    </Shape>
    <Shape>
      <Location left="225" top="458" />
      <Size width="253" height="107" />
    </Shape>
    <Shape>
      <Location left="225" top="372" />
      <Size width="99" height="50" />
    </Shape>
    <Shape>
      <Location left="5" top="458" />
      <Size width="207" height="107" />
    </Shape>
    <Shape>
      <Location left="5" top="575" />
      <Size width="207" height="93" />
    </Shape>
    <Shape>
      <Location left="225" top="575" />
      <Size width="253" height="93" />
    </Shape>
    <Shape>
      <Location left="5" top="689" />
      <Size width="207" height="122" />
    </Shape>
    <Connection>
      <StartNode isHorizontal="True" location="35" />
      <EndNode isHorizontal="True" location="35" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="65" />
      <EndNode isHorizontal="False" location="106" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="45" />
      <EndNode isHorizontal="True" location="45" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="42" />
      <EndNode isHorizontal="True" location="42" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="65" />
      <EndNode isHorizontal="False" location="66" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="44" />
      <EndNode isHorizontal="True" location="44" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="44" />
      <EndNode isHorizontal="True" location="44" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="46" />
      <EndNode isHorizontal="True" location="46" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="42" />
      <EndNode isHorizontal="True" location="42" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="38" />
      <EndNode isHorizontal="True" location="21" />
    </Connection>
  </Positions>
</ClassProject>