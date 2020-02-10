import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
export class Audit extends Component {
    constructor(props){
		super(props);
		this.state = {
		
		};
	};

	render(){
		const { site } = this.props;
		return (
			<div id="descriptionContent">
				<h3> FreeEDR Senior Design Audit Page </h3>
			</div>
		)
	}

}