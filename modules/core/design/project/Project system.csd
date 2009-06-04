<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Interface">
      <Name>IProject</Name>
      <Access>Public</Access>
      <Location left="963" top="581" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IComponentBuilder</Name>
      <Access>Public</Access>
      <Location left="1683" top="1622" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IObjectLoader</Name>
      <Access>Public</Access>
      <Location left="1673" top="1783" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectExtension</Name>
      <Access>Public</Access>
      <Location left="1636" top="581" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>The project system is responsible for tracking the project tree which holds the connections between the different 'data units'. Creation of sub-unit, sibbling unit etc. always runs through project!

</Text>
      <Location left="10" top="10" />
      <Size width="282" height="147" />
    </Entity>
    <Entity type="Interface">
      <Name>IDataSetGraph</Name>
      <Access>Public</Access>
      <Location left="1178" top="920" />
      <Size width="227" height="78" />
      <Collapsed>False</Collapsed>
      <Member type="Property">IDataSetGraphNode Root { get; }</Member>
    </Entity>
    <Entity type="Interface">
      <Name>IGraphNodeCreator</Name>
      <Access>Public</Access>
      <Location left="1444" top="677" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IDataSet</Name>
      <Access>Public</Access>
      <Location left="1879" top="1267" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Note that the 'data-unit' graph only needs to hold the 'description' of each initial data set. The descriptions hold all the variables, geometry and other data which can be changed to obtain another 'data-unit'. 

The generator, visualization and actual data sets can be linked to the 'data-unit'-description but they don't necessarily need to be in the tree.

By separating the actual generated data from the 'description' data we can allow modification (copy/clone/modify/edit etc.) of a 'data-unit' without having to digg through all the generated data. This also forces components / users to indicate what data should be considered in the tree and what data doesn't need to be considered.</Text>
      <Location left="10" top="180" />
      <Size width="282" height="261" />
    </Entity>
    <Entity type="Interface">
      <Name>IDataSetGraphNode</Name>
      <Access>Public</Access>
      <Location left="1178" top="1267" />
      <Size width="162" height="61" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandCentral&lt;IProjectCommand&gt;</Name>
      <Access>Public</Access>
      <Location left="931" top="272" />
      <Size width="255" height="66" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IVisualizer</Name>
      <Access>Public</Access>
      <Location left="1933" top="902" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IDataStorage</Name>
      <Access>Public</Access>
      <Location left="2315" top="902" />
      <Size width="162" height="66" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IGenerator</Name>
      <Access>Public</Access>
      <Location left="1673" top="902" />
      <Size width="162" height="66" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommand</Name>
      <Access>Public</Access>
      <Location left="1442" top="122" />
      <Size width="162" height="64" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectCommand</Name>
      <Access>Public</Access>
      <Location left="1477" top="272" />
      <Size width="162" height="63" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandCentral&lt;T&gt;</Name>
      <Access>Public</Access>
      <Location left="982" top="122" />
      <Size width="162" height="66" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Think about the relation between the generator and the project extention. Should all generators extend the project?</Text>
      <Location left="1819" top="481" />
      <Size width="220" height="86" />
    </Entity>
    <Entity type="Interface">
      <Name>IExtensionCommandCentral&lt;IProjectCommand&gt;</Name>
      <Access>Public</Access>
      <Location left="1140" top="481" />
      <Size width="313" height="69" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IHistory</Name>
      <Access>Public</Access>
      <Location left="2508" top="1057" />
      <Size width="162" height="76" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>There's no need to create a pooling system for these as we expect that nodes will not be created often, given that each node contains a complete data set.</Text>
      <Location left="1413" top="802" />
      <Size width="231" height="89" />
    </Entity>
    <Entity type="Comment">
      <Text>Each command can indicate if it is public or not. Non-publics can only be used by the project and the creator type.</Text>
      <Location left="1764" top="257" />
      <Size width="188" height="83" />
    </Entity>
    <Entity type="Interface">
      <Name>ITimeLineProxy</Name>
      <Access>Public</Access>
      <Location left="586" top="902" />
      <Size width="162" height="66" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>The project data is also stored in the project graph as the root node. So a project doesn't have any data itself.</Text>
      <Location left="867" top="839" />
      <Size width="214" height="80" />
    </Entity>
    <Entity type="Comment">
      <Text>Holds the commands that are accessable to the different project extensions</Text>
      <Location left="1254" top="330" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Interface">
      <Name>IProjectBuilder</Name>
      <Access>Public</Access>
      <Location left="647" top="360" />
      <Size width="162" height="81" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectService</Name>
      <Access>Public</Access>
      <Location left="632" top="554" />
      <Size width="162" height="80" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandCentralProxy</Name>
      <Access>Public</Access>
      <Location left="338" top="539" />
      <Size width="162" height="83" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IComponentService</Name>
      <Access>Public</Access>
      <Location left="1915" top="1509" />
      <Size width="178" height="78" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IDataSetStore</Name>
      <Access>Public</Access>
      <Location left="1095" top="710" />
      <Size width="193" height="78" />
      <Collapsed>False</Collapsed>
      <Member type="Property">IDataSet ProjectData { get; }</Member>
    </Entity>
    <Entity type="Comment">
      <Text>Only the data is stored in the individual data set files. All component related information is stored in the project file.</Text>
      <Location left="1636" top="1033" />
      <Size width="206" height="83" />
    </Entity>
    <Entity type="Interface">
      <Name>ILinkedComponentStore</Name>
      <Access>Public</Access>
      <Location left="2284" top="1509" />
      <Size width="162" height="88" />
      <Collapsed>False</Collapsed>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Association" first="1" second="2">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1764</X>
        <Y>1726</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1754</X>
        <Y>1722</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="5" second="6">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1317</X>
        <Y>889</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1415</X>
        <Y>706</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="5" second="9">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="9" second="9">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1260</X>
        <Y>1356</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1120</X>
        <Y>1297</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="9" second="7">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1376</X>
        <Y>1297</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1852</X>
        <Y>1297</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="10">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="3">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1189</X>
        <Y>615</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1580</X>
        <Y>615</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="12">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2086</X>
        <Y>1294</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2396</X>
        <Y>1021</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="13">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1910</X>
        <Y>1232</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1862</X>
        <Y>914</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="11">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1954</X>
        <Y>1206</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2018</X>
        <Y>1005</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Generalization" first="13" second="3">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Generalization" first="11" second="3">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1972</X>
        <Y>860</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1847</X>
        <Y>609</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="15" second="14">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1537</X>
        <Y>247</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1530</X>
        <Y>212</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="10" second="15">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1255</X>
        <Y>303</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1451</X>
        <Y>303</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="3" second="15">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1709</X>
        <Y>556</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1589</X>
        <Y>361</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Generalization" first="10" second="16">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="16" second="14">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1192</X>
        <Y>152</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1391</X>
        <Y>152</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="13" second="12">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>2388</X>
        <Y>800</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="11" second="12">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2014</X>
        <Y>866</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2350</X>
        <Y>832</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="17" second="3">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1768</X>
        <Y>512</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1767</X>
        <Y>556</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="3" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1597</X>
        <Y>598</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="18">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1074</X>
        <Y>556</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1113</X>
        <Y>522</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="12" second="19">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2521</X>
        <Y>938</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2588</X>
        <Y>998</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Comment" first="20" second="6">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="21" second="15">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1711</X>
        <Y>302</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1683</X>
        <Y>302</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="0" second="22">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>908</X>
        <Y>629</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>778</X>
        <Y>934</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Comment" first="23" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>974</X>
        <Y>671</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="24" second="18">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="18" second="10">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1115</X>
        <Y>503</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1075</X>
        <Y>370</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="22" second="19">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>655</X>
        <Y>1016</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2640</X>
        <Y>1436</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="26" second="0">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>871</X>
        <Y>602</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>913</X>
        <Y>602</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="26" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="26" second="25">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>706</X>
        <Y>510</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>706</X>
        <Y>467</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="26" second="27">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>589</X>
        <Y>582</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>534</X>
        <Y>582</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="7">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1832</X>
        <Y>1311</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1918</X>
        <Y>1357</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="29" second="5">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="29">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1047</X>
        <Y>750</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Comment" first="30" second="7">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1888</X>
        <Y>1194</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="28" second="31">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2143</X>
        <Y>1533</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2212</X>
        <Y>1533</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="28" second="1">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1840</X>
        <Y>1526</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="28" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2055</X>
        <Y>1612</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2001</X>
        <Y>1639</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>True</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
  </Relations>
</ClassProject>