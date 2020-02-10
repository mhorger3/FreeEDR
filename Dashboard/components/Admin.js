import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
export class Admin extends Component {
    constructor(props){
		super(props);
		this.state = {
		
		};
	};

	render(){
		const { site } = this.props;
		return (
			<div id="descriptionContent">
				<h3> FreeEDR Senior Design Admin Page </h3>
			</div>
		)
	}

}