import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
import {Select, InputLabel, MenuItem, Grid, Button, AppBar, Tab, Tabs, Radio, RadioGroup, FormHelperText, FormControlLabel, FormControl, FormLabel, TextField} from '@material-ui/core';
import { createMuiTheme, MuiThemeProvider } from '@material-ui/core';
import {FreeEDRIconButton} from '../components/FreeEDRIconButton.js';
export class Support extends Component {
    constructor(props){
		super(props);
		this.state = {
			value: 0,
			radio: 'New Access Request',
			support: "Low - 4"
		};
		this.handleChange = this.handleChange.bind(this);
		this.handleRadio = this.handleRadio.bind(this);
		this.handleSelect = this.handleSelect.bind(this);
	};

	handleChange(event, value){
	    this.setState({
	    	 value: value 
		});
	}

	handleRadio(event, value){
		this.setState({
			radio: value
		});
	}

	handleSelect(event, value){
		this.setState({
			support: event.target.value
		});
	}

	render(){
		const { site } = this.props;
		const { value, radio, support} = this.state;
		return (
			<div id="descriptionContent">
				<h3> FreeEDR Senior Design Support Page </h3>
				<AppBar position="static">
				<Tabs value={value} onChange={this.handleChange} aria-label="simple tabs example">
					<Tab label="Contact Incident Support"/>
					<Tab label="Support Staff Information"/>
					<Tab label="Submit a New Request"/>
				</Tabs>
				</AppBar>
				{value === 0 && <div id="content">
				<Grid container	spacing={0}	direction="column"	alignItems="center"	justify="center">
					<br></br>
					<br></br>
					<TextField id="textFieldName" label="Domain Name" value="FreeEDR/mhorger" size="large" style = {{width: 800}}/>
					<br></br>
					<br></br>
					<TextField id="textFieldEmail" label="Email" size="large" style = {{width: 800}} />
					<br></br>
					<br></br>
					<TextField id="textFieldDate" label="Date" size="large" style = {{width: 800}} />
					<br></br>
					<br></br>
					<InputLabel id="selectSupportStaff">Critical Level</InputLabel>
					<Select labelId="selectSupportStaff" id="selectSupportStaff-id"	value={support} style = {{width: 800}} 	onChange={this.handleSelect}>
						<MenuItem value="Low - 4">Low</MenuItem>
						<MenuItem value="Medium - 3">Medium</MenuItem>
						<MenuItem value="High - 2">High</MenuItem>
						<MenuItem value="Severe - 1">Severe</MenuItem>
					</Select>
					<br></br><br></br>	<br></br>
					<TextField id="textFieldRequest" id="outlined-multiline-static"
					label="Incident Information"
					multiline
					rows="4"
					defaultValue="Please put your support information here."
					variant="outlined" style = {{width: 800}}/>
					<br></br>
					<Button variant="contained" color="secondary">
						Upload Screenshots
					</Button>
					<br></br><br></br>
					<Button variant="contained" color="primary">
						Submit Incident
					</Button>
					</Grid>   			
				</div>}
				{value === 1 && <div id="content">
					<h3> Incident Response Manager: Mark Hamill </h3>
					<h5>&emsp;Email: mh1928@freeEDR.com, Phone: 866-847-1690, Desk: W16-90</h5>
					<br></br>
					<h3> Incident Response Supporter: Ed Helms </h3>
					<h5>&emsp;Email: eh2228@freeEDR.com, Phone: 866-847-1624, Desk: W16-24</h5>
					<br></br>
					<h3> System Auditor: Michael Scott </h3>
					<h5>&emsp;Email: ms0918@freeEDR.com, Phone: 866-847-1658, Desk: W16-58</h5>
					<br></br>
					<h3> Dashboard Infrastructure Manager: Mark Hamill  </h3>
					<h5>&emsp;Email: mh1928@freeEDR.com, Phone: 866-847-1690, Desk: W16-90</h5>
				</div>}
				{value === 2 && <div id="content">
				<br></br>
				<Grid container	spacing={0}	direction="column"	alignItems="center"	justify="center">
					<FormControl component="fieldset">
						<FormLabel component="legend">Request Type</FormLabel>
						<RadioGroup row aria-label="Request Type" name="RequestType" value={radio} onChange={this.handleRadio}>
						<FormControlLabel
							value="New Access Request"
							control={<Radio color="primary" />}
							label="New Access Request"
							labelPlacement="start"
						/>
						<FormControlLabel
							value="New Feature Request"
							control={<Radio color="primary" />}
							label="New Feature Request"
							labelPlacement="start"
						/>
						</RadioGroup>
					</FormControl>	
					<br></br>
					<br></br>
					<TextField id="textFieldName" label="Domain Name" value="FreeEDR/mhorger" size="large" style = {{width: 800}}/>
					<br></br>
					<br></br>
					<TextField id="textFieldEmail" label="Email" size="large" style = {{width: 800}} />
					<br></br>
					<br></br>
					<TextField id="textFieldDate" label="Date" size="large" style = {{width: 800}} />
					<br></br>
					<br></br>
					<TextField id="textFieldName" label="Request Type" value={radio} size="large" style = {{width: 800}} />
					<br></br><br></br>	<br></br>
					<TextField id="textFieldRequest" label="Request Information" id="outlined-multiline-static"
					label="Request Information"
					multiline
					rows="4"
					defaultValue="Please put your request information here."
					variant="outlined" style = {{width: 800}}/>
					<br></br><br></br>
					<Button variant="contained" color="primary">
						Submit Request
					</Button>
					</Grid>   			
				</div>}
			</div>
		)
	}

}