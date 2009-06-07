<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Class">
      <Name>BootStrapper</Name>
      <Access>Public</Access>
      <Location left="292" top="221" />
      <Size width="162" height="94" />
      <Collapsed>False</Collapsed>
      <Member type="Method">public void Start()</Member>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Interface">
      <Name>IAppDomainBuilder</Name>
      <Access>Public</Access>
      <Location left="710" top="261" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Could we do a fluent API for the core system? That will allow easy additions too (think extention methods)?</Text>
      <Location left="1005" top="1475" />
      <Size width="190" height="76" />
    </Entity>
    <Entity type="Comment">
      <Text>Loads the User Interface service. All other services will be started from the dependencies for the UI service</Text>
      <Location left="270" top="77" />
      <Size width="204" height="69" />
    </Entity>
    <Entity type="Interface">
      <Name>INeedStartup</Name>
      <Access>Public</Access>
      <Location left="437" top="1135" />
      <Size width="354" height="112" />
      <Collapsed>False</Collapsed>
      <Member type="Event">event EventHandler&lt;StartupProgressEventArgs&gt; StartupProgress</Member>
      <Member type="Method">void Start()</Member>
    </Entity>
    <Entity type="Comment">
      <Text>Needs way to do:
- error stuff
- async stuff
- Indicate complete</Text>
      <Location left="543" top="1355" />
      <Size width="206" height="102" />
    </Entity>
    <Entity type="Comment">
      <Text>Forms the front end of the project sub-system</Text>
      <Location left="1502" top="699" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>All communication between services runs through the message pipeline</Text>
      <Location left="1472" top="373" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Interface">
      <Name>IOwnServices</Name>
      <Access>Public</Access>
      <Location left="437" top="750" />
      <Size width="236" height="175" />
      <Collapsed>False</Collapsed>
      <Member type="Method">void Install(IService service)</Member>
      <Member type="Method">void Uninstall(IService service)</Member>
      <Member type="Method">void Start(IService service)</Member>
      <Member type="Method">void Start(Type serviceType)</Member>
      <Member type="Method">void Stop(IService service)</Member>
      <Member type="Method">void Restart(IService service)</Member>
    </Entity>
    <Entity type="Comment">
      <Text>Services have dependencies which have to be loaded before the service can be loaded.

A service is not normally running. Once a message for the service arrives it will be loaded and the message can be processed. 
Some services will be started on application start up.</Text>
      <Location left="1005" top="1297" />
      <Size width="187" height="160" />
    </Entity>
    <Entity type="Interface">
      <Name>ICommand</Name>
      <Access>Public</Access>
      <Location left="2132" top="261" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IMessage</Name>
      <Access>Public</Access>
      <Location left="1847" top="423" />
      <Size width="162" height="71" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandMessage</Name>
      <Access>Public</Access>
      <Location left="1847" top="261" />
      <Size width="162" height="74" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Timeline service will have to track blocking events (e.g. removal of a plug-in, replacement of a plug-in etc.) because these events make it impossible to follow the timeline backwards or forwards</Text>
      <Location left="1813" top="1275" />
      <Size width="243" height="92" />
    </Entity>
    <Entity type="Comment">
      <Text>Allows the core system to easily interact with the different services on an equal level. Mainly use to get to the message pipeline.</Text>
      <Location left="986" top="328" />
      <Size width="209" height="83" />
    </Entity>
    <Entity type="Comment">
      <Text>Need some way of allowing additional services to be started without needing to rely on the dependencies coming from the UI service</Text>
      <Location left="517" top="77" />
      <Size width="256" height="66" />
    </Entity>
    <Entity type="Comment">
      <Text>Could create a single class for services (KernelService or something) which is then responsible for starting / maintaining all the stuff behind the scenes. This would allow loading stuff into different AppDomains etc.</Text>
      <Location left="1230" top="1545" />
      <Size width="190" height="136" />
    </Entity>
    <Entity type="Comment">
      <Text>This also needs to do license verification?</Text>
      <Location left="76" top="232" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Class">
      <Name>KernelService</Name>
      <Access>Public</Access>
      <Location left="1230" top="1105" />
      <Size width="302" height="153" />
      <Collapsed>False</Collapsed>
      <Member type="Property">public abstract ServiceType ServiceType { get; }</Member>
      <Member type="Method">public abstract IEnumerable&lt;Type&gt; Dependencies()</Member>
      <Member type="Method">public abstract void RecieveMessage(KernelMessage message)</Member>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>MarshalByRef</Name>
      <Access>Public</Access>
      <Location left="1287" top="1355" />
      <Size width="162" height="82" />
      <Collapsed>False</Collapsed>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>UserInterfaceService</Name>
      <Access>Public</Access>
      <Location left="2140" top="1366" />
      <Size width="162" height="96" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>ProjectService</Name>
      <Access>Public</Access>
      <Location left="1472" top="822" />
      <Size width="162" height="92" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>TimelineService</Name>
      <Access>Public</Access>
      <Location left="1897" top="1135" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>MessagePipeline</Name>
      <Access>Public</Access>
      <Location left="1472" top="506" />
      <Size width="162" height="84" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>LogSink</Name>
      <Access>Public</Access>
      <Location left="812" top="1366" />
      <Size width="162" height="90" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>PluginRepository</Name>
      <Access>Public</Access>
      <Location left="1005" top="853" />
      <Size width="162" height="122" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>CoreProxy</Name>
      <Access>Public</Access>
      <Location left="1005" top="476" />
      <Size width="162" height="112" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>ServiceProvider</Name>
      <Access>Public</Access>
      <Location left="437" top="410" />
      <Size width="253" height="129" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Comment" first="3" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="5" second="4">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>646</X>
        <Y>1276</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="12" second="11">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="12" second="10">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="15" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>439</X>
        <Y>196</Y>
      </BendPoint>
    </Relation>
    <Relation type="Realization" first="18" second="4">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1191</X>
        <Y>1188</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>820</X>
        <Y>1188</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="18" second="19">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="9" second="18">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1190</X>
        <Y>1233</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="2" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1228</X>
        <Y>1506</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1251</X>
        <Y>1283</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="16" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1445</X>
        <Y>1634</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1477</X>
        <Y>1293</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="13" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="6" second="21">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="21" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1568</X>
        <Y>939</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1841</X>
        <Y>1163</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="20" second="22">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2115</X>
        <Y>1394</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2084</X>
        <Y>1183</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="20" second="23">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2111</X>
        <Y>1378</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2111</X>
        <Y>560</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="20" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2070</X>
        <Y>1410</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1501</X>
        <Y>1379</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="21" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1433</X>
        <Y>885</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1433</X>
        <Y>1063</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="22" second="23">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2096</X>
        <Y>1159</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1659</X>
        <Y>575</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="21" second="23">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1436</X>
        <Y>857</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1430</X>
        <Y>567</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="22" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1870</X>
        <Y>1190</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1582</X>
        <Y>1190</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="23" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1446</X>
        <Y>556</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1395</X>
        <Y>1080</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="24" second="18">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1143</X>
        <Y>1209</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="25" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1206</X>
        <Y>905</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1295</X>
        <Y>1076</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="26" second="18">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>958</X>
        <Y>536</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1193</X>
        <Y>1150</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="25" second="23">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1192</X>
        <Y>893</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1390</X>
        <Y>538</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="26" second="23">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1215</X>
        <Y>524</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1324</X>
        <Y>524</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="7" second="23">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="14" second="26">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Realization" first="27" second="8">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Realization" first="27" second="4">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>403</X>
        <Y>504</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>395</X>
        <Y>1190</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="27" second="26">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>757</X>
        <Y>506</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>971</X>
        <Y>506</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="27">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>385</X>
        <Y>344</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>406</X>
        <Y>476</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="17" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="27" second="1">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>672</X>
        <Y>296</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
  </Relations>
</ClassProject>