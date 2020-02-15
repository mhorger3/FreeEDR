import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
import {AppBar, Tab, Tabs} from '@material-ui/core';
export class Audit extends Component {
    constructor(props){
		super(props);
		this.state = {
			value: 0
		};
		this.handleChange = this.handleChange.bind(this);
	};

	handleChange(event, value){
	    this.setState({
	    	 value: value 
		});
	}

	render(){
		const { site } = this.props;
		const { value } = this.state;
		return (
			<div id="descriptionContent">
				<h3> FreeEDR Senior Design Audit Page </h3>
				<AppBar position="static">
				<Tabs value={value} onChange={this.handleChange} aria-label="simple tabs example">
					<Tab label="Audit Menu"/>
					<Tab label="SOC1 Audit Report"/>
					<Tab label="Internal Controls Audit"/>
					<Tab label="Workstation Audit"/>
					<Tab label="View Previous Audits"/>
				</Tabs>
				</AppBar>
				{value === 0 && <div id="content">a </div>}
				{value === 1 && <div id="content">b  </div>}
				{value === 2 && <div id="content">c  </div>}
			</div>
		)
	}

}