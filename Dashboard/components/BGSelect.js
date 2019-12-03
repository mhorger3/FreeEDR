import React, { Component } from 'react';
import { Select, MuiThemeProvider, createMuiTheme } from '@material-ui/core';

export class BGSelect extends Component {
	render(){
		const {children, id, value, name, onChange, style} = this.props;
		return (
			<Select value={value} onChange={onChange} id={id} name={name} style={style}>
				{children}
			</Select>
		)
	}

}