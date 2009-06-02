<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Interface">
      <Name>IService</Name>
      <Access>Public</Access>
      <Location left="1087" top="527" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Class">
      <Name>BootStrapper</Name>
      <Access>Public</Access>
      <Location left="168" top="688" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Interface">
      <Name>IAppDomainBuilder</Name>
      <Access>Public</Access>
      <Location left="465" top="288" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IUserInterfaceService</Name>
      <Access>Public</Access>
      <Location left="865" top="674" />
      <Size width="162" height="96" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectService</Name>
      <Access>Public</Access>
      <Location left="1275" top="688" />
      <Size width="162" height="86" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IPluginRepository</Name>
      <Access>Public</Access>
      <Location left="939" top="95" />
      <Size width="162" height="126" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>How do we deal with commands here? Services will need to add their commands to the system.</Text>
      <Location left="1711" top="542" />
      <Size width="177" height="79" />
    </Entity>
    <Entity type="Interface">
      <Name>IMessagePipeline</Name>
      <Access>Public</Access>
      <Location left="1289" top="274" />
      <Size width="162" height="126" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandService</Name>
      <Access>Public</Access>
      <Location left="1406" top="489" />
      <Size width="162" height="139" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Could we do a fluent API for the core system? That will allow easy additions too (think extention methods)?</Text>
      <Location left="1711" top="423" />
      <Size width="190" height="76" />
    </Entity>
    <Entity type="Interface">
      <Name>ILogSink</Name>
      <Access>Public</Access>
      <Location left="748" top="213" />
      <Size width="162" height="131" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Loads the different parts of the system.</Text>
      <Location left="168" top="542" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Interface">
      <Name>INeedStartup</Name>
      <Access>Public</Access>
      <Location left="415" top="674" />
      <Size width="329" height="112" />
      <Collapsed>False</Collapsed>
      <Member type="Event">event EventHandler&lt;StartupProgressEventArgs&gt; Progress</Member>
    </Entity>
    <Entity type="Comment">
      <Text>Needs way to do:
- error stuff
- async stuff
- Indicate complete</Text>
      <Location left="520" top="983" />
      <Size width="206" height="102" />
    </Entity>
    <Entity type="Comment">
      <Text>Forms the front end of the project sub-system</Text>
      <Location left="1633" top="700" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Where does history come into all of this? Both UI and Project will need a history. We'll need to stitch these two together somehow.</Text>
      <Location left="10" top="10" />
      <Size width="220" height="107" />
    </Entity>
    <Entity type="Comment">
      <Text>All communication between services runs through the message pipeline</Text>
      <Location left="814" top="396" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Interface">
      <Name>IOwnServices</Name>
      <Access>Public</Access>
      <Location left="477" top="514" />
      <Size width="162" height="84" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Services have dependencies which have to be loaded before the service can be loaded.

A service is not normally running. Once a message for the service arrives it will be loaded and the message can be processed. 
Some services will be started on application start up.</Text>
      <Location left="1052" top="781" />
      <Size width="187" height="160" />
    </Entity>
    <Entity type="Interface">
      <Name>ITimeLineService</Name>
      <Access>Public</Access>
      <Location left="1252" top="127" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>How do we deal with dependencies? Using messages only is to limited? Use commands directly?</Text>
      <Location left="1052" top="1052" />
      <Size width="171" height="106" />
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Generalization" first="5" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1125</X>
        <Y>163</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1152</X>
        <Y>500</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="7" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1262</X>
        <Y>358</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1214</X>
        <Y>451</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="6" second="8">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Generalization" first="10" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>935</X>
        <Y>294</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1130</X>
        <Y>502</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="11" second="1">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="1" second="12">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>249</X>
        <Y>805</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>554</X>
        <Y>856</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="13" second="12">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>623</X>
        <Y>839</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="14" second="4">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1586</X>
        <Y>732</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1494</X>
        <Y>726</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="8" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1378</X>
        <Y>562</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1301</X>
        <Y>562</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="4" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1228</X>
        <Y>617</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="3" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1111</X>
        <Y>617</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="16" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1040</X>
        <Y>430</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1111</X>
        <Y>491</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="17" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>704</X>
        <Y>545</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1045</X>
        <Y>545</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Comment" first="9" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1660</X>
        <Y>447</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1230</X>
        <Y>502</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="17" second="12">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Generalization" first="0" second="12">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>684</X>
        <Y>571</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>684</X>
        <Y>641</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="18" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Generalization" first="19" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1210</X>
        <Y>164</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1194</X>
        <Y>500</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="20" second="18">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
  </Relations>
</ClassProject>