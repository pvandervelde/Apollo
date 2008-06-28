<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Class">
      <Name>ComponentId</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Categories</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Tags</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Capabilies</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Constraints</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>SubObjects</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Schedule</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>ExternalInterfaces</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>State</Name>
      <Access>Public</Access>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>A component is just a collection of objects. There is no actual instance of the component. Don't use individual objects to create a component. Maybe create a single component interface / controller object.

Components to connect to other components --&gt; Let them figure out connections by themselves.

Note that some elements are manditory but others are not. Some components may have all the elements while others may not have them.</Text>
    </Entity>
    <Entity type="Comment">
      <Text>The external interfaces and the capabilities are related. Connect them somehow.</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Constrains and external interfaces are related. Connect them somehow.</Text>
    </Entity>
    <Entity type="Class">
      <Name>InstructionSets</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Categories stores the categories for this component</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Tags stores the tags for this component</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Stores the instruction sets for this component. Instructions for:
- Construction
- Destruction</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Allow constructions like:
- Action1: Invoke [METHOD] on [OBJECT] with id [NUMBER]
</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Do we need communication lanes? Define them for:
- Inter component
- Inter object
- Component - UI and reverse</Text>
    </Entity>
    <Entity type="Comment">
      <Text>External interfaces describe the different "APIs" that are available to other components</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Mark sub-objects with special tags indicating if they are part of the external interface or not?</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Stores the state for a single component. Only state is persisted</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Are categories also tags? Probably. Maybe use a taxonomy?</Text>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Association" first="3" second="7">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="4" second="7">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="5">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="10" second="3" />
    <Relation type="Comment" first="11" second="4" />
    <Relation type="Comment" first="13" second="1" />
    <Relation type="Comment" first="14" second="2" />
    <Relation type="Comment" first="15" second="12" />
    <Relation type="Comment" first="18" second="7" />
    <Relation type="Comment" first="19" second="5" />
    <Relation type="Comment" first="20" second="8" />
    <Relation type="Comment" first="21" second="1" />
    <Relation type="Comment" first="21" second="2" />
    <Relation type="Comment" first="16" second="12" />
  </Relations>
  <Positions>
    <Shape>
      <Location left="3" top="208" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="805" top="3" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="805" top="96" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="582" top="393" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="805" top="393" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="708" top="716" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="3" top="300" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="708" top="577" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="3" top="393" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="3" top="3" />
      <Size width="294" height="178" />
    </Shape>
    <Shape>
      <Location left="440" top="487" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="970" top="487" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="805" top="198" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="1046" top="3" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="1046" top="96" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="1046" top="198" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="406" top="208" />
      <Size width="332" height="50" />
    </Shape>
    <Shape>
      <Location left="3" top="654" />
      <Size width="199" height="93" />
    </Shape>
    <Shape>
      <Location left="972" top="577" />
      <Size width="162" height="78" />
    </Shape>
    <Shape>
      <Location left="972" top="716" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="226" top="393" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="580" top="3" />
      <Size width="162" height="72" />
    </Shape>
    <Connection>
      <StartNode isHorizontal="False" location="101" />
      <EndNode isHorizontal="True" location="27" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="111" />
      <EndNode isHorizontal="True" location="23" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="77" />
      <EndNode isHorizontal="False" location="77" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="67" />
      <EndNode isHorizontal="True" location="36" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="38" />
      <EndNode isHorizontal="True" location="43" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="36" />
      <EndNode isHorizontal="True" location="36" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="34" />
      <EndNode isHorizontal="True" location="34" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="43" />
      <EndNode isHorizontal="True" location="43" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="50" />
      <EndNode isHorizontal="True" location="50" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="36" />
      <EndNode isHorizontal="True" location="36" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="32" />
      <EndNode isHorizontal="True" location="32" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="39" />
      <EndNode isHorizontal="True" location="39" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="112" />
      <EndNode isHorizontal="True" location="40" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="32" />
      <EndNode isHorizontal="True" location="42" />
    </Connection>
  </Positions>
</ClassProject>