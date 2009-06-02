<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Class">
      <Name>ComponentId</Name>
      <Access>Public</Access>
      <Location left="3" top="208" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Categories</Name>
      <Access>Public</Access>
      <Location left="805" top="3" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Tags</Name>
      <Access>Public</Access>
      <Location left="805" top="96" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Capabilies</Name>
      <Access>Public</Access>
      <Location left="582" top="393" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Constraints</Name>
      <Access>Public</Access>
      <Location left="805" top="393" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>SubObjects</Name>
      <Access>Public</Access>
      <Location left="708" top="716" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Schedule</Name>
      <Access>Public</Access>
      <Location left="3" top="300" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>ExternalInterfaces</Name>
      <Access>Public</Access>
      <Location left="708" top="577" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>State</Name>
      <Access>Public</Access>
      <Location left="3" top="393" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>A component is just a collection of objects. There is no actual instance of the component. Don't use individual objects to create a component. Maybe create a single component interface / controller object.

Components to connect to other components --&gt; Let them figure out connections by themselves.

Note that some elements are manditory but others are not. Some components may have all the elements while others may not have them.</Text>
      <Location left="3" top="3" />
      <Size width="294" height="178" />
    </Entity>
    <Entity type="Comment">
      <Text>The external interfaces and the capabilities are related. Connect them somehow.</Text>
      <Location left="440" top="487" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Constrains and external interfaces are related. Connect them somehow.</Text>
      <Location left="970" top="487" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Class">
      <Name>InstructionSets</Name>
      <Access>Public</Access>
      <Location left="805" top="198" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Categories stores the categories for this component</Text>
      <Location left="1046" top="3" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Tags stores the tags for this component</Text>
      <Location left="1046" top="96" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Stores the instruction sets for this component. Instructions for:
- Construction
- Destruction</Text>
      <Location left="1046" top="198" />
      <Size width="162" height="85" />
    </Entity>
    <Entity type="Comment">
      <Text>Allow constructions like:
- Action1: Invoke [METHOD] on [OBJECT] with id [NUMBER]
</Text>
      <Location left="406" top="208" />
      <Size width="332" height="50" />
    </Entity>
    <Entity type="Comment">
      <Text>Do we need communication lanes? Define them for:
- Inter component
- Inter object
- Component - UI and reverse</Text>
      <Location left="3" top="654" />
      <Size width="199" height="93" />
    </Entity>
    <Entity type="Comment">
      <Text>External interfaces describe the different "APIs" that are available to other components</Text>
      <Location left="972" top="577" />
      <Size width="162" height="78" />
    </Entity>
    <Entity type="Comment">
      <Text>Mark sub-objects with special tags indicating if they are part of the external interface or not?</Text>
      <Location left="972" top="716" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Stores the state for a single component. Only state is persisted</Text>
      <Location left="226" top="393" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Are categories also tags? Probably. Maybe use a taxonomy?</Text>
      <Location left="582" top="3" />
      <Size width="162" height="72" />
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Association" first="3" second="7">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>683</X>
        <Y>503</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>683</X>
        <Y>604</Y>
      </BendPoint>
      <Direction>Bidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="4" second="7">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>916</X>
        <Y>503</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>895</X>
        <Y>600</Y>
      </BendPoint>
      <Direction>Bidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="5">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>785</X>
        <Y>687</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>785</X>
        <Y>691</Y>
      </BendPoint>
      <Direction>Bidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="10" second="3">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>507</X>
        <Y>462</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>557</X>
        <Y>429</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="11" second="4">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1008</X>
        <Y>462</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>992</X>
        <Y>436</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="13" second="1">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1021</X>
        <Y>39</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>992</X>
        <Y>39</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="14" second="2">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1021</X>
        <Y>130</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>992</X>
        <Y>130</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="15" second="12">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1021</X>
        <Y>241</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>992</X>
        <Y>241</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="18" second="7">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>947</X>
        <Y>627</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>895</X>
        <Y>627</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="19" second="5">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>947</X>
        <Y>752</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>895</X>
        <Y>752</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="20" second="8">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>201</X>
        <Y>425</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>190</X>
        <Y>425</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="21" second="1">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>769</X>
        <Y>42</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>780</X>
        <Y>42</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="21" second="2">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>694</X>
        <Y>100</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>780</X>
        <Y>136</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="16" second="12">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>763</X>
        <Y>240</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>780</X>
        <Y>240</Y>
      </BendPoint>
    </Relation>
  </Relations>
</ClassProject>